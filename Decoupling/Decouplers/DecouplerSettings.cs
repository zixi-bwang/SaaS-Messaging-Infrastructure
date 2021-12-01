namespace Decoupling.Decouplers
{
    public class DecouplerSettings
    {
        public DecouplerIdentitySettings Identity { get; set; }
        public string[] Assemblies { get; set; }
        public bool SkipSetupDb { get; set; }
    }
}
