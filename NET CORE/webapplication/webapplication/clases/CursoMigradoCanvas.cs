
namespace webapplication.clases
{
    public class CursoMigradoCanvas
    {
        public CursoCanvas curso { get; set; }
        public ModuloCanvas modulo { get; set; }
        public ItemCanvas item { get; set; }

        public string? url { get; set; }

        public string? ind_multicarrera { get; set; }
        public PageCanvas? page { get; set; }
    }
}
