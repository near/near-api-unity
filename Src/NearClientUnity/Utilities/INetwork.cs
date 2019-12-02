namespace NearClientUnity.Utilities
{
    public interface INetwork
    {
        string Name { get; }
        string ChainId { get; }
        dynamic DefaultProvider(dynamic providers);
    }
}