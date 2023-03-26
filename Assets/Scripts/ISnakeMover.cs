using System.Collections;

namespace DefaultNamespace
{
    public interface ISnakeMover
    {
        public void StartMoving();

        public void IncreaseBody();

        public IEnumerator MoveRoutine();
    }
}