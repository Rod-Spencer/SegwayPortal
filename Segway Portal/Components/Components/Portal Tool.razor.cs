using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Segway.EF.SegwayCntxt;

namespace Segway_Portal.Components.Components
{
    public partial class Portal_Tool : ComponentBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Constructors
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Parameters

        [Parameter]
        public PortalServiceTool? PortalTool { get; set; }

        [Parameter]
        public PortalServiceToolData? PortalData { get; set; }

        [Parameter]
        public List<PortalToolInstallerType>? InstallerTypes { get; set; }

        [Parameter]
        public List<PortalToolState>? PortalStates { get; set; }

        [Parameter]
        public List<PortalToolType>? PortalToolTypes { get; set; }

        [Parameter]
        public EventCallback<PortalServiceTool?> OnPortalToolChanged { get; set; }

        [Parameter]
        public EventCallback<PortalServiceToolData?> OnPortalDataChanged { get; set; }

        [Parameter]
        public EventCallback<Boolean> OnShowPortalToolPageChanged { get; set; }

        #endregion Parameters
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Injections
        #endregion Injections
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Public Properties

        public Boolean ShowPortalToolPage { get; set; } = false;

        #endregion Public Properties
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Screen State Properties
        #endregion Screen State Properties
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Override Methods
        #endregion Override Methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Protected Methods
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Public Methods

        public async Task ShowPopup()
        {
            ShowPortalToolPage = true;
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Private Methods


        private async Task ClosePopup()
        {
            ShowPortalToolPage = false;
            await OnShowPortalToolPageChanged.InvokeAsync(false);
        }

        private async Task SaveTool()
        {
            ShowPortalToolPage = false;
            await OnPortalDataChanged.InvokeAsync(PortalData);
            await OnPortalToolChanged.InvokeAsync(PortalTool);
            await OnShowPortalToolPageChanged.InvokeAsync(true);
        }

        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file is null)
            {
                PortalData?.FileData = null;
                return;
            }

            List<String> parts = e.File.Name.Split('.').ToList();

            if (parts.Count == 8)
            {
                PortalTool!.DisplayName = parts[0].Trim();
                PortalTool.Version = String.Join('.', parts.GetRange(2, 4));
                var upType = parts[1].Trim().Split('-').ToList();
                PortalTool.InstallerTypeId = InstallerTypes?.FirstOrDefault(x => x.Description == upType[0].Trim())?.Id;

                PortalTool!.DisplayName = $"{parts[0].Trim()} V{String.Join('.', parts.GetRange(2, 2))} - {upType[0].Trim()}";
            }
            else if (parts.Count == 2)
            {
                PortalTool!.DisplayName = parts[0].Trim();
            }

            else if (parts.Count == 3)
            {
                PortalTool!.DisplayName = $"{parts[0].Trim()}.{parts[1].Trim()}";
            }

            // Update the model with the selected file name
            PortalTool!.FileName = file.Name;

            // Optional: read the file stream if you need the content
            using (var stream = file.OpenReadStream(Int32.MaxValue))
            {
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    PortalData!.FileData = ms.ToArray();
                    PortalData!.FileLength = PortalData!.FileData.Length;
                }
            }


            StateHasChanged();
        }


        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////
    }

}
