namespace Entatea
{
    public enum DataContextState : byte
    {
        NoTransaction = 0,

        InTransaction = 1,

        Committed = 2,

        Rolledback = 3
    }
}
