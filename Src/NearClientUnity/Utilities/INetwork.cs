namespace NearClientUnity.Utilities
{
    public interface INetwork
    {
        string Name { get; set; }
        string ChainId { get; set; }
        dynamic DefaultProvider(dynamic providers);
    }
}