using System.Collections.Generic;

namespace MazeGame.Maze
{
    public class Branch<T>
    {
        public List<T> Path { get; private set; }
        public Branch<T> SourceBranch { get; private set; } // TODO
        public T Source { get; private set; } // TODO
        public BranchType Type { get; private set; }


        public Branch(List<T> path, BranchType type, Branch<T> sourceBranch = default, T source = default)
        {
            Path = path;
            SourceBranch = sourceBranch;
            Source = source;
            Type = type;
        }
    }

    public enum BranchType
    {
        Main,
        Secondary,
    }
}