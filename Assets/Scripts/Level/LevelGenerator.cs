using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Text;
using UnityEngine.Tilemaps;
using NUnit.Framework;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;

public class LevelGenerator : MonoBehaviour
{

    struct Islet
    {
        public int height, width;
        public Vector2Int pos;
        public Islet(int h, int w, Vector2Int position)
        {
            height = h;
            width = w;
            pos = position;
        }
    }
    /*
     * levelWidth : 64
levelHeight : 64
seed : 42
num of islets : 15
minIsletHeight : 2
maxIsletHeight : 9
minIsletWidth : 2
minIsletWidth : 9
space between islets : 3
maxIterations : 100
protrusionrollmax : 255
protrusionrollThreshold: 140
maxYProtrusion : 3
maxXProtrusion : 3
     * 
     * */

    [SerializeField] int levelWidth = 64;
    [SerializeField] int levelHeight = 64;
    [SerializeField] int seed = 42;
    [SerializeField] int levelNumber = 0;
    [SerializeField] int numOfIslets = 2;
    [SerializeField] int minIsletHeight = 2;
    [SerializeField] int maxIsletHeight = 9;
    [SerializeField] int minIsletWidth = 2;
    [SerializeField] int maxIsletWidth = 9;
    [SerializeField] int spaceBetweenIslets = 3;
    [SerializeField] int maxIterations = 100;
    [SerializeField] int protrusionRollMax = 255;
    [SerializeField] int protrusionRollThreshold = 64;
    [SerializeField] int maxXProtrusion = 5;
    [SerializeField] int maxYProtrusion = 5;
    [SerializeField] bool protuse = true;

    [SerializeField] Tilemap groundTilemap;
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] Tilemap decorationsTilemap;
    [SerializeField] Tilemap outlineTileMap;

    [SerializeField] RuleTile groundTile;
    [SerializeField] RuleTile wallTile;

    string[] rules;
    char[,] levelGrid;

    public Dictionary<String, List<Vector2Int>> illegalPatterns = new();


    [ContextMenu("Clear tilemaps")]
    public void ClearTileMaps(){

        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        decorationsTilemap.ClearAllTiles();
        outlineTileMap.ClearAllTiles();

    }


    private static readonly Vector2Int[] neighborDirections =
    {
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 0),
        new Vector2Int(1,1),
        };



[ContextMenu("Generate level")]
    public void Generate(){

        UnityEngine.Random.InitState(seed);
        string fileName = "Assets/Generated/level" + levelNumber + ".txt";
        levelGrid = new char[levelWidth, levelHeight];

        // Initialize all stars for starters
        for(int y = 0; y < levelHeight; ++y){
            for(int x = 0; x < levelWidth; ++x){
                levelGrid[x, y] = '*';
            }
        }

        // Fetch walls ruletile:

        string WallRuleTilePath = "Assets/Generated/WallRuleTile.txt";
        string text = File.ReadAllText(WallRuleTilePath);

        string[] mRules = text.Split(
            new[] { "\r\n\r\n", "\n\n" },
            StringSplitOptions.RemoveEmptyEntries);

        rules = mRules.Select(NormalizeRule).ToArray();

        generateWalls();
        generateIslets();
        if (protuse) {
            generateProtrusions();
        }

        char[,] previous = (char[,])levelGrid.Clone();
        int consecutiveIterationResults = 0;
        int broken = 0;
        for(int r = 0; r < maxIterations; ++r)
        {
            illegalPatterns.Clear();
            FindIllegalWallTiles();
            outputIllegalPatterns(illegalPatterns);
            broken = illegalPatterns.Values.Sum(x => x.Count);
            Debug.Log("Found " + broken + " illegal wall tiles");
            if (broken == 0) break;
            ApplyRepairIteration();
            if (GridsAreEqual(previous, levelGrid)){
                consecutiveIterationResults++;
            }
            if(consecutiveIterationResults > 3)
            {
                Debug.Log("Abort : " + consecutiveIterationResults + " consecutive results - algorithm needs to introduce suboptimal changes");
                break;
            }

        }
        

        OutputLevel(fileName, levelGrid);
        RenderLevel(levelGrid);
    }

    bool GridsAreEqual(char[,] a, char[,] b)
    {
       if (a.GetLength(0) != b.GetLength(0) ||
       a.GetLength(1) != b.GetLength(1))
            return false;

        for (int y = 0; y < a.GetLength(1); ++y)
        {
            for (int x = 0; x < a.GetLength(0); ++x)
            {
                if (a[x, y] != b[x, y])
                    return false;
            }
        }
        return true;
    }



    void generateWalls()
    {
        // default walls : length and witdh of two encompassing the level
        for (int y = 0; y < levelHeight; ++y)
        {
            for (int x = 0; x < levelWidth; ++x)
            {
                if (y < 2 || y + 2 >= levelHeight ||
                   x < 2 || x + 2 >= levelWidth)
                {
                    levelGrid[x, y] = '#';
                }
            }
        }

    }
    /* Iterate through the array
 * roll for a chance to create protrusion on a tile that has a neighboring #
 * roll again for the type of protrusion, then roll and loop for how much protrusion there is
 * (alternatively, create protrusion in both direction once the core loop works?)
 */
    void generateProtrusions()
    {
        int protrusionX = UnityEngine.Random.Range(2, maxXProtrusion);
        int protrusionY = UnityEngine.Random.Range(2, maxYProtrusion);
        int roll = UnityEngine.Random.Range(0, protrusionRollMax);
        int totalProtrusions = 0;

        for (int y = 2; y < levelHeight - 2; ++y)
        {
            for (int x = 2; x < levelWidth - 2; ++x)
            {

                if (roll > protrusionRollThreshold)
                {

                    // check for neighboring '#' tiles
                    Vector2Int pos = new Vector2Int(x, y);

                    int neighborCount = 0;
                    List<Vector2Int> non_wall_neighbors = new();
                    foreach (Vector2Int v in neighborDirections)
                    {
                        if (levelGrid[x + v.x, y + v.y] == '#')
                        {
                            neighborCount++;
                            
                        }
                        else
                        {
                            non_wall_neighbors.Add(v);
                        }
                    }

                    if (neighborCount < 1) continue;
                    // UPDATE : Find suitable direction for protrusion?
                    

                    for (int proty = y; proty < y + protrusionY; ++proty)
                        {
                            for (int protx = x; protx < protrusionX; ++protx)
                            {
                                if(proty < levelHeight && protx < levelWidth)
                            {
                                levelGrid[protx, proty] = '#';
                            }
                                
                            }
                        }


                    protrusionX = UnityEngine.Random.Range(2, maxXProtrusion);
                    protrusionY = UnityEngine.Random.Range(2, maxYProtrusion);
                    ++totalProtrusions;
                }

                roll = UnityEngine.Random.Range(0, protrusionRollMax);
                
            }

        }
        Debug.Log("Succesful protrusions: " + totalProtrusions +" times ");
    }
    /*
    * x ranging from 3+islet_width to _width -1 -islet_Width (+3 to disallow non-passable path between wall edge and islet edge)
    * y ranging from 3+islet_height to height -1 -isletHeight (+3 can be made to a serializable if desired)
    * generate one random x and y
    * generate additional islets by validating the minimum distance
    * iterate through max iterations or until the islets are placed

    NOTE : The theorycrafing up did not lead to a fruitful resolution, and likely an additional sweep needs to be done to preserve the tilemap constraints.
    */
    void generateIslets()
    {

        int isletsPlaced = 0;
        int iterations = 0;
        List<Islet> islets = new List<Islet>();

        int x, y;
        float epsi = 0.01f;
        List<Vector2Int> isletPositions = new List<Vector2Int>();


        while (isletsPlaced <= numOfIslets && iterations < maxIterations)
        {

            // generate new x, new y
            // loop through the existing islet locations, and try to get a non-clashing x and y
            int isletHeight = UnityEngine.Random.Range(minIsletHeight, maxIsletHeight);
            int isletWidth = UnityEngine.Random.Range(minIsletWidth, maxIsletWidth);

            x = UnityEngine.Random.Range(3 + isletHeight, levelHeight - 1 - isletHeight);
            y = UnityEngine.Random.Range(3 + isletWidth, levelWidth - 1 - isletWidth);

            // if the distance between x + width and pos.x - width is less than space between, reroll for new x
            // repeat for y
            bool isClash = false;
            foreach(Islet other in islets){
                int x1 = x + isletWidth;
                int x2 = other.pos.x + other.width;
                double dx = (x1 - x2) * (x1 - x2);
                double distx = Math.Sqrt(dx);
                if (Math.Abs(distx - spaceBetweenIslets) <= epsi){
                    isClash = true;
                    break;
                }

                int y1 = y + isletHeight;
                int y2 = other.pos.y + other.height;
                double dy = (y1 - y2) * (y1 - y2);
                double disty = Math.Sqrt(dy);
                if (Math.Abs(disty - spaceBetweenIslets) <= epsi){
                    isClash = true;
                    break;
                }
            }
            if(!isClash){
                for (int j = y; j < y + isletHeight; ++j)
                {
                    for (int i = x; i < x + isletWidth; ++i)
                    {
                        levelGrid[i, j] = '#';
                    }
                }
                islets.Add(new Islet(isletHeight, isletWidth, new Vector2Int(x, y)));
                ++isletsPlaced;
                ++iterations;
            }
            

        }
        Debug.Log("Iterations used " + iterations + ", produced :" + isletsPlaced + " islets");
      
    }



    bool MatchesRule(string pattern, string rule)
    {
        for(int i = 0; i < 8; ++i)
        {
            char r = rule[i];
            if (r == '_' || r == 'C') continue;

            if (pattern[i] != r)
            {
                return false;
            }
        }
        return true;
    }

    bool MatchesAnyRule(string pattern)
    {
       
        foreach(string rule in rules)
        {
            if (MatchesRule(pattern, rule))
            {
                if (rule == "____C____") return false; // the catch all pattern , that the ruleset does not know how to render
                return true;
            }
        }

        return false;
    }

    string GetPattern(int x, int y)
    {
        StringBuilder sb = new();

        foreach (var d in neighborDirections)
        {
            sb.Append(levelGrid[x + d.x, y + d.y]);
        }
        return sb.ToString();
    }

    string NormalizeRule(string rule)
    {
        return rule
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace(" ", "");
    }


    void FindIllegalWallTiles()
    {
        int illegals = 0;

        foreach (var rule in rules)
        {
            for (int i = 0; i < rule.Length; i++)
            {
                Debug.Log($"{i}: '{rule[i]}' ({(int)rule[i]})");
            }
        }


        for(int y = 1; y < levelHeight - 1; ++y)
        {
            for(int x = 1; x < levelWidth - 1; ++x)
            {
                if (levelGrid[x, y] != '#') continue;

                string pattern = GetPattern(x, y);

                if (!MatchesAnyRule(pattern))
                {
                    if (!illegalPatterns.ContainsKey(pattern))
                    {
                        illegalPatterns[pattern] = new List<Vector2Int>();
                    }
                    illegalPatterns[pattern].Add(new Vector2Int(x, y));
                    illegals++;
                }
            }
        }

        Debug.Log("Encountered " + illegals + " illegal pattern instances");

    }

    void outputIllegalPatterns(Dictionary<String,List<Vector2Int>> illegalPatterns)
    {
        string fileName = "Assets/Generated/level" + levelNumber + "illegals.txt";

        StringBuilder output = new();

        foreach(var kv in illegalPatterns)
        {
            //int occurrences = kv.Value.Count();
            output.Append($"{kv.Value.Count()} occurences\n{PrettyPattern(kv.Key)}\n");
            foreach(var v in kv.Value)
            {
                output.Append("(" + v.x + "," + v.y + ")\n");
            }
            output.AppendLine();
        }
        File.WriteAllText(fileName, output.ToString());
    }

    string PrettyPattern(string p)
    {
        if (p.Length != 8)
            return $"Invalid pattern ({p.Length}): {p}";

        return
            $"{p[0]}{p[1]}{p[2]}\n" +
            $"{p[3]}C{p[4]}\n" +
            $"{p[5]}{p[6]}{p[7]}";
    }



    bool IsIllegalNeighborhood(Vector2Int v)
    {


        foreach (Vector2Int n in neighborDirections)
        {
            int x = v.x + n.x;
            int y = v.y + n.y;

            if (x < 0 || x >= levelWidth ||
                y < 0 || y >= levelHeight)
                return false;
        }

        string s = "";
        foreach(Vector2Int n in neighborDirections)
        {
            s += levelGrid[v.x +n.x, v.y + n.y];
        }
        return MatchesAnyRule(s);
    }



    int CountIllegalNeighborhoods(Vector2Int node)
    {
        int illegalCount = 0;
        foreach(Vector2Int v in neighborDirections)
        {
            Vector2Int neighorLocation = new Vector2Int(node.x + v.x, node.y + v.y);
            if (IsIllegalNeighborhood(neighorLocation)) illegalCount++;
        }
        return illegalCount;
    }


    int SimulateDeletion(Vector2Int node)
    {
        int initialIllegalNeighborhoods = CountIllegalNeighborhoods(node);
        levelGrid[node.x, node.y] = '*';

        int resultingIllegalNeighborhoods = CountIllegalNeighborhoods(node);
        
        levelGrid[node.x, node.y] = '#';
        return initialIllegalNeighborhoods - resultingIllegalNeighborhoods;

    }

    int GetHammingDistance(string a, string b)
    {
        int d = 0;
        for(int i = 0; i < a.Length - 1; ++i)
        {
            if (a[i] != b[i]) ++d;
        }
        return d;
    }



    /**
    * Edited the algorithm to choose randomly between all suitable candidates and solve the tie randomly.
    */
    (int score, string closestMatchingPattern) SimulateAddition(Vector2Int node)
    {
        int initialIllegalNeighborhoods = CountIllegalNeighborhoods(node);
        String invalidPattern = GetPattern(node.x, node.y);
        List<string> allMatchingCandidates = new();
        int minimimumHammingDistance = 8;
        int currentHammingDistance = 0;
        for(int i = 0; i < rules.Length; ++i)
        {
            currentHammingDistance = GetHammingDistance(rules[i], invalidPattern);
            if (currentHammingDistance <= minimimumHammingDistance)
            {
                minimimumHammingDistance = currentHammingDistance;
                allMatchingCandidates.Add(rules[i]);
            }
        }

        List<(string pattern, int illegalCount)> candidateIntrusions = new();
        // On a tie case, this iterates more than once
        foreach (string patternCandidate in allMatchingCandidates)
        {
            // Apply changes to grid
            int iter = 0;
            foreach (Vector2Int v in neighborDirections)
            {
                Vector2Int pos = new Vector2Int(node.x + v.x, node.y + v.y);
                if (iter == 4) levelGrid[pos.x, pos.y] = '#';
                else levelGrid[pos.x, pos.y] = patternCandidate[iter];
                ++iter;
            }
            candidateIntrusions.Add((patternCandidate, CountIllegalNeighborhoods(node)));

            //restore the grid to compare it to next pattern candidate if any
            iter = 0;
            foreach (Vector2Int v in neighborDirections)
            {
                Vector2Int pos = new Vector2Int(node.x + v.x, node.y + v.y);
                if (iter == 4) levelGrid[pos.x, pos.y] = '#';
                else levelGrid[pos.x, pos.y] = invalidPattern[iter];
                ++iter;
            }
        }

        int best = candidateIntrusions.Min(c => c.illegalCount);
        var bestCandidates = candidateIntrusions.Where(c => c.illegalCount == best).ToList();
        var winner = bestCandidates[UnityEngine.Random.Range(0, bestCandidates.Count)];
        return (winner.illegalCount, winner.pattern);
    }

    
    enum RepairStrategy
    {
        Delete,CompletePattern
    }

    struct RepairCandidate
    {
        public Vector2Int Node;
        public RepairStrategy Strategy;
        public int RemaininingIllegal;
        public string ClosestMatchingPattern;
        // Idea : compare the edit distances as well as the remaining illegal neighborhoods for a more conservative approach

        public RepairCandidate( Vector2Int n,
         RepairStrategy strat,
         int remain,
         string closest)
        {
            Node = n;
            Strategy = strat;
            RemaininingIllegal = remain;
            ClosestMatchingPattern = closest;
        }

        public void ApplyStrategy(char[,] levelGrid)
        {
            switch(Strategy)
            {
                case RepairStrategy.Delete:
                    levelGrid[Node.x, Node.y] = '*';
                    break;
                case RepairStrategy.CompletePattern:
                    int i = 0;
                    foreach (var dir in neighborDirections)
                    {
                        Vector2Int pos = Node + dir;
                        if (i == 4) levelGrid[pos.x, pos.y] = '#';
                        else
                        {
                            if (ClosestMatchingPattern[i] == '_')
                            {
                                int roll = UnityEngine.Random.Range(0, 2);
                                if (roll == 1) levelGrid[pos.x, pos.y] = '*';
                                else levelGrid[pos.x, pos.y] = '#';
                            }
                            else
                            {
                                levelGrid[pos.x, pos.y] = ClosestMatchingPattern[i++];
                            }
                                
                        }
                            
                    }
                    break;
            }
        }
    }

    /**
     * Completes a single repair iteration.
     * Iteration is based on invasiveness idea : strategies are ranked on how many of the nodes neighbors become invalid.
       This idea borrows from medicine : the least invasive way of handling a medical procedure is tried before escalating
       to the next least invasive.

        Short description of algo:
     * 1) Iterates through the list of illegal wall tiles (global variable).
     * 2) For any given wall tile, compares two strategies : 
       2a) deleting the illegal node, and simulating its effect to the neighbors
       2b) comparing the pattern on the node for existing legal patterns and finds all with the lowest Hamming distance
       - If multiple fit, returns a random one of these with again the least invasive change gets priority
       
        3) on a tie case ( both strategies are equally good/bad), one gets picked based on the seed state ie next random number

       3) Compares the this way found best strategies with each other and applies the strategy.
        - on tie, filters all that have the lowest count and then chooses one, once again randomly.
     */
    void ApplyRepairIteration()
    {
        List<RepairCandidate> candidates = new();
        int initialIllegalPatterns = illegalPatterns.Values.Sum(x => x.Count);
        Debug.Log("Attempting to remove : " + initialIllegalPatterns + "illegal nodes");
        

        foreach (var kv in illegalPatterns)
        {
            foreach(var node in kv.Value)
            {
                int onDelete = SimulateDeletion(node);
                var additionResult = SimulateAddition(node);
                int onAddToNext = additionResult.score;
                if (onDelete < onAddToNext)
                {
                    candidates.Add(
                        new RepairCandidate(
                            node,
                            RepairStrategy.Delete,
                            onDelete,
                            ""
                        ));
                }
                else if (onAddToNext > onDelete)
                {
                    candidates.Add(new RepairCandidate(
                        node,
                        RepairStrategy.CompletePattern,
                        onAddToNext,
                        additionResult.closestMatchingPattern
                        ));
                }
                // tie breaker - this should be rather rare case.
                else
                {
                    int roll = UnityEngine.Random.Range(0, 2);
                    if(roll == 0)
                    {
                        candidates.Add(
                      new RepairCandidate(
                          node,
                          RepairStrategy.Delete,
                          onDelete,
                          ""
                      ));
                    }
                    else
                    {
                        candidates.Add(new RepairCandidate(
                        node,
                        RepairStrategy.CompletePattern,
                        onAddToNext,
                        additionResult.closestMatchingPattern
                        ));
                    }
                }

            }
        }

        int best = candidates.Min(c => c.RemaininingIllegal);
        var bestCandidates = candidates.Where(c => c.RemaininingIllegal == best).ToList();
        RepairCandidate winner = bestCandidates[UnityEngine.Random.Range(0, bestCandidates.Count)];
        winner.ApplyStrategy(levelGrid);

    }


   
    void OutputLevel(string filePath, char[,] levelGrid)
    {
        StringBuilder output = new StringBuilder();
        
        for (int y = 0; y < levelWidth; ++y)
        {
            for (int x = 0; x < levelHeight; ++x)
            {
                output.Append(levelGrid[x, y]);
            }
            output.AppendLine();
        }
        Debug.Log(output);

        output.Append("Config : ");
 

        output.Append("levelWidth : " + levelWidth + "\n");
        output.Append("levelHeight : " + levelHeight + "\n");
        output.Append("seed : " + seed + "\n");
        output.Append("num of islets : " + numOfIslets+ "\n");
        output.Append("minIsletHeight : " + minIsletHeight + "\n");
        output.Append("maxIsletHeight : " + maxIsletHeight + "\n");
        output.Append("minIsletWidth : " + minIsletWidth + "\n");
        output.Append("minIsletWidth : " + maxIsletWidth + "\n");

        output.Append("space between islets : " + spaceBetweenIslets + "\n");
        output.Append("maxIterations : " + maxIterations + "\n");
        output.Append("protrusionrollmax : " + protrusionRollMax+ "\n");
        output.Append("protrusionrollThreshold: " + protrusionRollThreshold+ "\n");
        output.Append("maxYProtrusion : " + maxYProtrusion + "\n");
        output.Append("maxXProtrusion : " + maxYProtrusion+ "\n");
        File.WriteAllText(filePath, output.ToString());
        
   
    }

    void RenderLevel(char[,] levelGrid)
    {
        ClearTileMaps();
        for(int y = 0; y < levelHeight; ++y)
        {
            for(int x = 0; x < levelWidth; ++x)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (levelGrid[x,y] == '#')
                {
                    wallTilemap.SetTile(cell, wallTile);
                }
                else
                {
                    groundTilemap.SetTile(cell, groundTile);
                }
            }
        }
       
        Tile debugTile = new Tile();
        debugTile.color = new Color(1, 0, 1);
        foreach (var kv in illegalPatterns)
        {
            foreach(var coordpair in kv.Value)
            {
                Vector3Int cell = new Vector3Int(coordpair.x, coordpair.y, 0);
                //wallTilemap.SetTile(cell, debugTile);
            }
        }


    }



}