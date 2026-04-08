using System;

    public interface IStateNameProvider
    {
        event Action<string> OnStateNameChanged;
    }