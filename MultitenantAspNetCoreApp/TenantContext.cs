namespace MultitenantAspNetCoreApp
{
    public interface ITenantContext
    {
        Tenant CurrentTenant { get; }
    }

    public interface ITenantSetter
    {
        Tenant CurrentTenant { set; }
    }

    public class TenantContext : ITenantContext, ITenantSetter
    {
        public Tenant CurrentTenant { get; set; }
    }

    public class Tenant
    {
        public string Name { get; set; }

        public Tenant(string name)
        {
            Name = name;
        }
    }
}