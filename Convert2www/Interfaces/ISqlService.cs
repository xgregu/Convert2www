namespace Convert2www.Interfaces
{
    internal interface ISqlService
    {
        void WareReader();

        void ConractorReader();

        bool IsSqlConnectionActive { get; }
    }
}