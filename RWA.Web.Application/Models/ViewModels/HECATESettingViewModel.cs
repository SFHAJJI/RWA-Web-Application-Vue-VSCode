
using System.ComponentModel.DataAnnotations;

namespace RWA.Web.Application.Models
{
    public class HECATESettingViewModel
    {

        [Required]
        public List<ImportExportViewModel> importExportViewModels { get; set; }
        // Detailed results of processing each product row
        public List<ImportResult> ImportResults { get; set; } = new List<ImportResult>();

        [Required]
        public string PageTitle { get; set; }

        public string UploadResultMessage { get; set; }

        public HECATESettingViewModel()
        {
            PageTitle = "Import/Export du param�trage";
            //UploadResultMessage = "Failed";
            importExportViewModels = new List<ImportExportViewModel>()
            {
                new ImportExportViewModel("BDD Historique", ImportExportType.BDDHistorique, " Le fichier doit �tre nomm� BDDHistorique.xlsx et contenir l'onglet B"),
                
                new ImportExportViewModel( "Mapping Cat�gorie RWA", ImportExportType.MappingCatRWA, "Le fichier doit �tre nomm� Parametrage RWA.xlsx et contenir les onglets CategorieRWA, TypeBloomberg et EquivalenceCatRWA")
            };
       

        }
    }
}
