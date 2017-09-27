using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Data.Json;

namespace File_Template_Centre
{
    class FileTemplate
    {
        private StorageFile data = null;
        private String templateName = null;
        private String dateCreated = null;
        private String notes = null;
        private bool[] allSet = { false, false, false, false };
        private const String MANIFEST_FILE = "templatemanifest.dat";

        public FileTemplate(StorageFile file)
        {
            data = file;
            allSet[0] = true;
            setupManifest();
        }

        public String getTemplateName()
        {
            return templateName;
        }

        public void setTemplateName(String templateName)
        {
            this.templateName = templateName;
            allSet[1] = true;
        }

        public String getDateCreated()
        {
            return dateCreated;
        }

        public void setDateCreated(String dateCreated)
        {
            this.dateCreated = dateCreated;
            allSet[2] = true;
        }

        public String getNotes()
        {
            return notes;
        }

        public void setNotes(String notes)
        {
            this.notes = notes;
            allSet[3] = true;
        }

        //Save data to file and info to manifest
        public async void saveTemplate()
        {
            //check if all data members were set before trying to save template
            if (!isAllset())
                throw new System.NullReferenceException(
                    "All variables must be set to a value first before saving."
                    );
            /* get filename 
             - displayName is assumed to be filename */
            String templateName = data.DisplayName;
            //get manifest file to append additional template info to
            StorageFile templateManifest = 
                await ApplicationData.Current.LocalCacheFolder.GetFileAsync(MANIFEST_FILE);
            if (templateManifest == null)
                throw new System.NullReferenceException(
                    "templateManifest was set to null. manifest file has not been created."
                    );
            String line = templateName + "," + dateCreated + "," + notes + "\n";
            await FileIO.AppendLinesAsync(templateManifest, new List<String> { line });
            //create file to save actual template file data
            StorageFile localFile = 
                await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(templateName,
                CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(localFile, await FileIO.ReadTextAsync(data));
        }

        //check if all data members are set
        private bool isAllset()
        {
            foreach (bool b in allSet)
            {
                if (b is false)
                    return false;
            }
            return true;
        }

        //create manifest file is not already created
        private async void setupManifest()
        {
            StorageFile manifest = 
                await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(
                    MANIFEST_FILE, 
                    CreationCollisionOption.FailIfExists
                    );
            if (manifest != null)
            {
                await FileIO.WriteTextAsync(manifest, "");
            }
        }
    }
}
