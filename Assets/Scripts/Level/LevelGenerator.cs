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
using System.Net.Http.Headers;

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

    [SerializeField] int MinLevelWidth = 24;
    [SerializeField] int MaxLevelWidth = 100;
    [SerializeField] int MinLevelHeight = 24;
    [SerializeField] int MaxLevelHeight = 136;

    [SerializeField] int seed = 42;
    [SerializeField] int levelNumber = 0;

    [SerializeField] int MinIsletCount = 1;
    [SerializeField] int MaxIsletCount = 2;
    [SerializeField] int minIsletHeight = 2;
    [SerializeField] int maxIsletHeight = 9;
    [SerializeField] int minIsletWidth = 2;
    [SerializeField] int maxIsletWidth = 9;
    [SerializeField] int MinSpaceBetweenIslets = 3;


    [SerializeField] int MaxRepairIterations = 100;
    [SerializeField] int MaxIsletGenerationIterations = 100;
    [SerializeField] int protrusionRollMax = 255;
    [SerializeField] int protrusionRollThreshold = 64;
    [SerializeField] int MinXProtrusion = 1;
    [SerializeField] int MinYProtrusion = 1;

    [SerializeField] int MaxXProtrusion = 5;
    [SerializeField] int MaxYProtrusion = 5;

    [SerializeField] bool protrudeIslets = true;
    [SerializeField] bool protrudeOuterWalls = true;

    int levelWidth, levelHeight, numOfIslets, IsletGenerationIterationsUsed, RepairIterationsUsed;

    string[] rules;
    char[,] levelGrid;

    public Dictionary<String, List<Vector2Int>> illegalPatterns = new();
    List<Islet> islets = new();


    private static readonly Vector2Int[] patternCoordinates =
{
    new(-1,  1),
    new( 0,  1),
    new( 1,  1),

    new(-1,  0),
    new( 0,  0),
    new( 1,  0),

    new(-1, -1),
    new( 0, -1),
    new( 1, -1)
};


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



    /**
     * Generator operates on the assumption, that the ruletile patterns are already exported.
     * For future versions it would be nice to find a less hardcoded solution
     */

    string levelOutputFilename,
        levelConfigOutputFilename,
        levelRemainingIllegalPatternsFilename,
        levelInfoOutputFilename;
[ContextMenu("Generate level")]
    public void Generate(){

        UnityEngine.Random.InitState(seed);
        levelOutputFilename = "Assets/Generated/Working/level" + levelNumber + ".txt";
        levelConfigOutputFilename = "Assets/Generated/Working/level" + levelNumber + "config.txt";
        levelInfoOutputFilename = "Assets/Generated/Working/level" + levelNumber + "info.txt";

        levelWidth = UnityEngine.Random.Range(MinLevelWidth,MaxLevelWidth);
        levelHeight = UnityEngine.Random.Range(MinLevelHeight, MaxLevelHeight);

        numOfIslets = UnityEngine.Random.Range(MinIsletCount, MaxIsletCount);
        levelGrid = new char[levelWidth, levelHeight];

        // Initialize all stars for starters
        for(int y = 0; y < levelHeight; ++y){
            for(int x = 0; x < levelWidth; ++x){
                levelGrid[x, y] = '*';
            }
        }

        // Fetch walls ruletile from the default hardcoded path where the pattern extractor tries to place it
        string WallRuleTilePath = "Assets/Generated/WallRuleTile.txt";
        string text = File.ReadAllText(WallRuleTilePath);

        string[] mRules = text.Split(
            new[] { "\r\n\r\n", "\n\n" },
            StringSplitOptions.RemoveEmptyEntries);

        rules = mRules.Select(NormalizeRule).ToArray();

        generateWalls();
        IsletGenerationIterationsUsed = 0;
        generateIslets();

        if (protrudeIslets) ProtrudeIslets();
        if (protrudeOuterWalls) ProtrudeOuterWalls();
        
        

        char[,] previous = (char[,])levelGrid.Clone();
        int consecutiveIterationResults = 0;
        int broken = 0;
        RepairIterationsUsed = 0;
        while(RepairIterationsUsed < MaxRepairIterations)
        {
            illegalPatterns.Clear();
            GetIllegalPatterns();
            broken = illegalPatterns.Values.Sum(x => x.Count);
            if (broken == 0) break;
            ApplyRepairIteration();
            if (GridsAreEqual(previous, levelGrid)){
                consecutiveIterationResults++;
            }
            if(consecutiveIterationResults > 3)
            {
                break;
            }
            RepairIterationsUsed++;
        }

        if(consecutiveIterationResults > 3)
        {
            if(illegalPatterns.Count > 0)
            {
                OutputIllegalPatterns();
                // algorithm did not converge with this amount of repair iterations - check generated illegal patterns file for deeper analysis
                return;
            }
        }
        

        OutputLevel();
        OutputLevelConfig();
        OutputLevelInfo();
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

 
    void ProtrudeOuterWalls()
    {
        int protrusionX, protrusionY;
        // protrude top wall down
        for(int x = 2; x < levelWidth -2; ++x)
        {
            int roll = UnityEngine.Random.Range(0, protrusionRollMax);
            if(roll > protrusionRollThreshold)
            {
                protrusionY = UnityEngine.Random.Range(MinYProtrusion, MaxYProtrusion);
                int startingY = levelHeight - 3;
                for(int y = startingY; y > startingY - protrusionY; --y)
                {
                    levelGrid[x, y] = '#';
                }
            }
        }

        // protrude left wall to right
        for(int y = 2; y < levelHeight - 2; ++y)
        {
            int roll = UnityEngine.Random.Range(0, protrusionRollMax);
            if(roll > protrusionRollThreshold)
            {
                protrusionX = UnityEngine.Random.Range(MinXProtrusion, MaxXProtrusion);
                for(int x = 2; x < protrusionX + 2; ++x)
                {
                    levelGrid[x, y] = '#';
                }
            }
        }

        // protrude right wall to left
        for (int y = 2; y < levelHeight - 2; ++y)
        {
            int roll = UnityEngine.Random.Range(0, protrusionRollMax);
            if (roll > protrusionRollThreshold)
            {
                protrusionX = UnityEngine.Random.Range(MinXProtrusion, MaxXProtrusion);
                int startingX = levelWidth - 2;
                for(int x = startingX; x > startingX - protrusionX; --x)
                {
                    levelGrid[x, y] = '#';
                }
            }
        }

        // protrude bottom wall up
        for (int x = 2; x < levelWidth - 2; ++x)
        {
            int roll = UnityEngine.Random.Range(0, protrusionRollMax);
            if (roll > protrusionRollThreshold)
            {
                protrusionY = UnityEngine.Random.Range(MinYProtrusion, MaxYProtrusion);
                for (int y = 2; y < protrusionY + 2; ++y)
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
    void ProtrudeIslets()
    {
        int protrusionX;
        int protrusionY;
      


        foreach (Islet i in islets)
        {
            int roll = UnityEngine.Random.Range(0, protrusionRollMax);
            if (roll > protrusionRollThreshold)
            {

                protrusionX = UnityEngine.Random.Range(MinXProtrusion, MaxXProtrusion);
                protrusionY = UnityEngine.Random.Range(MinYProtrusion, MaxYProtrusion);

                // Maybe this will save some headache, don't know.
                if (i.pos.x + i.width + protrusionX > levelWidth
                    || i.pos.x - i.width - protrusionX < 0
                    || i.pos.y + i.height + protrusionY > levelHeight
                    || i.pos.y - i.height - protrusionY < 0) continue;


                int rollX_Or_Y = UnityEngine.Random.Range(0, 2);
                if (rollX_Or_Y == 1)
                {
                    int rollXDirection = UnityEngine.Random.Range(0, 2);
                    int y = i.pos.y - UnityEngine.Random.Range(1, i.height);
                    int whichAdjacentY = UnityEngine.Random.Range(0, 2);
                    int adjacentY = whichAdjacentY == 0 ? 1 : -1;
                    if (rollXDirection == 1)
                    {
                        // protrude right
                        int x = i.pos.x + i.width;

                        for (int iter = 0; iter < protrusionX; ++iter)
                        {
                            levelGrid[x + iter, y] = '#';
                            levelGrid[x + iter, y + adjacentY] = '#';
                        }
                    }
                    else
                    {
                        int x = i.pos.x;
                        //protrude left
                        for (int iter = 0; iter > protrusionX; ++iter)
                        {
                            levelGrid[x - iter, y] = '#';
                            levelGrid[x - iter, y + adjacentY] = '#';
                        }
                    }
                }
                else
                {
                    int rollYDirection = UnityEngine.Random.Range(0, 2);
                    int x = i.pos.x + UnityEngine.Random.Range(1, i.width);
                    int whichAdjacentX = UnityEngine.Random.Range(0, 2);
                    int adjacentX = whichAdjacentX == 0 ? 1 : -1;
                    if (rollYDirection == 1)
                    {
                        //protrude up
                        int y = i.pos.y;
                        for(int iter = 0; iter < protrusionY; ++iter)
                        {
                            levelGrid[x, y+iter] = '#';
                            levelGrid[x + adjacentX, y+iter] = '#';
                        }
                    }
                    else
                    {
                        //protrude down
                        int y = i.pos.y - i.height;
                        for(int iter = 0; iter < protrusionY; ++iter)
                        {
                            levelGrid[x, y-iter] = '#';
                            levelGrid[x + adjacentX, y - iter] = '#';
                        }
                        
                    }

                }


            }
        }

    }

    void generateIslets()
    {

        int isletsPlaced = 0;
        int x, y;
        float epsi = 0.01f;
        List<Vector2Int> isletPositions = new List<Vector2Int>();
        while (isletsPlaced < numOfIslets && IsletGenerationIterationsUsed < MaxIsletGenerationIterations)
        {

            // generate new x, new y
            // loop through the existing islet locations, and try to get a non-clashing x and y
            int isletHeight = UnityEngine.Random.Range(minIsletHeight, maxIsletHeight +1);
            int isletWidth = UnityEngine.Random.Range(minIsletWidth, maxIsletWidth +1);

            x = UnityEngine.Random.Range(3 + isletHeight, levelWidth - 1 - isletWidth);
            y = UnityEngine.Random.Range(3 + isletHeight, levelHeight - 1 - isletHeight);

            // if the distance between x + width and pos.x - width is less than space between, reroll for new x
            // repeat for y
            bool isClash = false;
            foreach(Islet other in islets){
                int x1 = x + isletWidth;
                int x2 = other.pos.x + other.width;
                double dx = (x1 - x2) * (x1 - x2);
                double distx = Math.Sqrt(dx);
                if (Math.Abs(distx - MinSpaceBetweenIslets) <= epsi){
                    isClash = true;
                    break;
                }

                int y1 = y + isletHeight;
                int y2 = other.pos.y + other.height;
                double dy = (y1 - y2) * (y1 - y2);
                double disty = Math.Sqrt(dy);
                if (Math.Abs(disty - MinSpaceBetweenIslets) <= epsi){
                    isClash = true;
                    break;
                }
            }
            if(!isClash){
                if (y < 0 || y + isletHeight >= levelHeight) continue;
                if (x < 0 || x + isletWidth >= levelWidth) continue;
                for (int j = y; j < y + isletHeight; ++j)
                {
                    
                    for (int i = x; i < x + isletWidth; ++i)
                    {
                        
                        levelGrid[i, j] = '#';
                    }
                }
                islets.Add(new Islet(isletHeight, isletWidth, new Vector2Int(x, y)));
                ++isletsPlaced;
                ++IsletGenerationIterationsUsed;
            }
        }

    }



    bool MatchesRule(string pattern, string rule)
    {
        for(int i = 0; i < 8; ++i)
        {
            char r = rule[i];
            if (r == '_') continue;

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
                if (rule == "____#____") return false; // the catch all pattern , that the ruleset does not know how to render
                return true;
            }
        }

        return false;
    }

    string GetPattern(int x, int y)
    {
        StringBuilder sb = new();

        foreach (var d in patternCoordinates)
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
            .Replace(" ", "")
            .Replace('C', '#');
    }


    void GetIllegalPatterns()
    {
        int illegals = 0;

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

    }

    void OutputIllegalPatterns()
    {
      
        StringBuilder output = new();
        output.Append("After " + RepairIterationsUsed + " repair iterations, the following list of illegal patterns remain \n");
        foreach(var kv in illegalPatterns)
        {
            output.Append($"{kv.Value.Count()} occurences\n{PrettyPattern(kv.Key)}\n");
            foreach(var v in kv.Value)
            {
                output.Append("(" + v.x + "," + v.y + ")\n");
            }
            output.AppendLine();
        }
        File.WriteAllText(levelRemainingIllegalPatternsFilename, output.ToString());
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





    (int score, string closestMatchingPattern) SimulateDeletion(Vector2Int node)
    {
        int initialIllegalNeighborhoods = CountIllegalNeighborhoods(node);
        String invalidPattern = GetPattern(node.x, node.y);
        List<string> allMatchingCandidates = new();
        int minimimumHammingDistance = 8;
        int currentHammingDistance = 0;
        for (int i = 0; i < rules.Length; ++i)
        {
            currentHammingDistance = GetRemovalDistance(rules[i], invalidPattern);
            if (currentHammingDistance <= minimimumHammingDistance)
            {
                minimimumHammingDistance = currentHammingDistance;
                allMatchingCandidates.Add(rules[i]);
            }
        }

        List<(string pattern, int illegalCount)> candidateIntrusions = new();

        foreach (string patternCandidate in allMatchingCandidates)
        {
            // Apply changes to grid
            int iter = 0;
            foreach (Vector2Int v in patternCoordinates)
            {
                Vector2Int pos = new Vector2Int(node.x + v.x, node.y + v.y);
                levelGrid[pos.x, pos.y] = patternCandidate[iter];
                ++iter;
            }
            candidateIntrusions.Add((patternCandidate, CountIllegalNeighborhoods(node)));

            //restore the grid to compare it to next pattern candidate if any
            iter = 0;
            foreach (Vector2Int v in patternCoordinates)
            {
                Vector2Int pos = new Vector2Int(node.x + v.x, node.y + v.y);
                levelGrid[pos.x, pos.y] = invalidPattern[iter];
                ++iter;
            }
        }

        int best = candidateIntrusions.Min(c => c.illegalCount);
        var bestCandidates = candidateIntrusions.Where(c => c.illegalCount == best).ToList();
        var winner = bestCandidates[UnityEngine.Random.Range(0, bestCandidates.Count)];
        return (winner.illegalCount, winner.pattern);

    }


    int GetRemovalDistance(string a, string b)
    {
        int d = 0;
        for(int i = 0; i < a.Length -1 ; ++i)
        {
            if (a[i] == '#' && b[i] == '#') ++d;
        }
        return d;
    }
  

    int GetAdditionDistance(string a, string b)
    {
        int d = 0;
        for (int i = 0; i < a.Length -1; ++i)
        {
            if (a[i] == '*' && b[i] == '#') ++d;
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
            currentHammingDistance = GetAdditionDistance(rules[i], invalidPattern);
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
            foreach (Vector2Int v in patternCoordinates)
            {
                Vector2Int pos = new Vector2Int(node.x + v.x, node.y + v.y);
                levelGrid[pos.x, pos.y] = patternCandidate[iter];
                ++iter;
            }
            candidateIntrusions.Add((patternCandidate, CountIllegalNeighborhoods(node)));

            //restore the grid to compare it to next pattern candidate if any
            iter = 0;
            foreach (Vector2Int v in patternCoordinates)
            {
                Vector2Int pos = new Vector2Int(node.x + v.x, node.y + v.y);
                levelGrid[pos.x, pos.y] = invalidPattern[iter];
                ++iter;
            }
        }

        int best = candidateIntrusions.Min(c => c.illegalCount);
        var bestCandidates = candidateIntrusions.Where(c => c.illegalCount == best).ToList();
        var winner = bestCandidates[UnityEngine.Random.Range(0, bestCandidates.Count)];
        return (winner.illegalCount, winner.pattern);
    }

    public char GetBestWildCardOption(Vector2Int pos)
    {
        levelGrid[pos.x, pos.y] = '#';
        int wallScore = CountIllegalNeighborhoods(pos);

        levelGrid[pos.x, pos.y] = '#';
        int floorScore = CountIllegalNeighborhoods(pos);

        return wallScore <= floorScore ? '#' : '*';

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

    }

    /**
     * Completes a single repair iteration.
     * Iteration is based on invasiveness idea : strategies are ranked on how many of the nodes neighbors become invalid.
       This idea borrows from medicine : the least invasive way of handling a medical procedure is tried before escalating
       to the next least invasive.

        Short description of algo:
     * 1) Iterates through the list of illegal wall tiles (global variable).
     * 2) For any given wall tile, compares two strategies : 
       2a) deleting wall nodes to reach a valid pattern
       2b) adding wall nodes to reach a valid pattern
       The least amount of additions or deletions are all collected, and on the case that additiondistance is same for multiple, a random one of those is returned. Deletions vice versa.
       
        3) on a tie case ( both strategies are equally good/bad), one gets picked based on the seed state ie next random number

       3) Compares the this way found best strategies with each other and applies the strategy.
        - If a wildcard is present in a pattern ('_') , the algoritm simulates both options and chooses the one that produces the least amount of illegal neighbors, observed from the wildcard node.
     */
    void ApplyRepairIteration()
    {
        List<RepairCandidate> candidates = new();
        int initialIllegalPatterns = illegalPatterns.Values.Sum(x => x.Count);
        foreach (var kv in illegalPatterns)
        {
            foreach(var node in kv.Value)
            {
                var deletionResult = SimulateDeletion(node);
                int onDelete = deletionResult.score;
                var additionResult = SimulateAddition(node);
                int onAddToNext = additionResult.score;
                if (onDelete < onAddToNext)
                {
                    candidates.Add(
                        new RepairCandidate(
                            node,
                            RepairStrategy.Delete,
                            onDelete,
                            deletionResult.closestMatchingPattern
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
                          deletionResult.closestMatchingPattern
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
        ApplyRepairCandidateStrategy(winner);

    }

    void ApplyRepairCandidateStrategy(RepairCandidate candidate)
    {

        int i = 0;
        foreach (var dir in patternCoordinates)
        {
            Vector2Int pos = candidate.Node + dir;
            if (candidate.ClosestMatchingPattern[i] == '_')
            {
                levelGrid[pos.x, pos.y] = GetBestWildCardOption(pos);
            }
            else
            {
                levelGrid[pos.x, pos.y] = candidate.ClosestMatchingPattern[i];
            }
            ++i;
        }
    }



    void OutputLevel()
    {
        StringBuilder output = new ();
        for (int y = 0; y < levelHeight; ++y)
        {
            for (int x = 0; x < levelWidth; ++x)
            {
                output.Append(levelGrid[x, y]);
            }
            output.AppendLine();
        }
        File.WriteAllText(levelOutputFilename, output.ToString());
        
    }

    void OutputLevelConfig()
    {
        StringBuilder configOutput = new();


        configOutput.Append("Config : \n");
        configOutput.Append("seed : " + seed + "\n");
        configOutput.Append("min level width :" + MinLevelWidth + "\n");
        configOutput.Append("max level width :" + MaxLevelWidth + "\n");
        configOutput.Append("min level height :" + MinLevelHeight + "\n");
        configOutput.Append("max level height :" + MaxLevelHeight + "\n");
        configOutput.Append("min number of islets : " + MinIsletCount + "\n");
        configOutput.Append("max number of islets : " + MaxIsletCount + "\n");
        configOutput.Append("minIsletHeight : " + minIsletHeight + "\n");
        configOutput.Append("maxIsletHeight : " + maxIsletHeight + "\n");
        configOutput.Append("minIsletWidth : " + minIsletWidth + "\n");
        configOutput.Append("maxIsletWidth : " + maxIsletWidth + "\n");
        configOutput.Append("space between islets : " + MinSpaceBetweenIslets + "\n");
        configOutput.Append("maxIterations : " + MaxRepairIterations + "\n");
        configOutput.Append("protrusionrollmax : " + protrusionRollMax + "\n");
        configOutput.Append("protrusionrollThreshold: " + protrusionRollThreshold + "\n");
        configOutput.Append("minXProtrusion : " + MinXProtrusion + "\n");
        configOutput.Append("minYProtrusion : " + MinYProtrusion + "\n");
        configOutput.Append("maxYProtrusion : " + MaxYProtrusion + "\n");
        configOutput.Append("maxXProtrusion : " + MaxYProtrusion + "\n");
        configOutput.Append("protrude islets : " + protrudeIslets + "\n");
        configOutput.Append("protrude outer walls : " + protrudeOuterWalls + "\n");
        File.WriteAllText(levelConfigOutputFilename, configOutput.ToString());
    }

    void OutputLevelInfo()
    {
        StringBuilder infoOutput = new();
        infoOutput.Append("width : " + levelWidth +"\n");
        infoOutput.Append("height : " + levelHeight + "\n");
        infoOutput.Append("seed:" + seed + "\n");
        infoOutput.Append("Islet count " + numOfIslets + "\n");
        File.WriteAllText(levelInfoOutputFilename, infoOutput.ToString());
    }





}