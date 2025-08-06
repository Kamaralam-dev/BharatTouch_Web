using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class ModalViewModel
    {
        public string ModalId { get; set; } = "defaultModal";
        public string ModalSize { get; set; } = "modal-lg";//
        // small -modal-sm
        // large- modal-lg
        // Extra Large- modal-xl
        // Fullscreen - modal-fullscreen
        public string ModalStyle { get; set; }
        public string Title { get; set; }
        public string BodyContent { get; set; } // Raw HTML content
        public string PartialViewName { get; set; } // Name of the partial view
        public object PartialViewData { get; set; } // Model for the partial view
        public string CloseButtonText { get; set; } = "Close";
        public string PrimaryButtonText { get; set; }
        public string PrimaryButtonOnClick { get; set; }
    }
}
