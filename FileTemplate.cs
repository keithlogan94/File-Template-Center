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
        private StorageFile fileData = null;
        private String localFileName = null;
        private String templateName = null;
        private String templateDescription = null;
        private String dateCreated = null;
        private bool[] allSet = { false, false, false, false, false };
        private const String MANIFEST_FILE = "templatemanifest.dat";
        private enum SetType { LOCAL_FILEDATA, LOCAL_FILENAME, TEMPLATE_NAME, DATE_CREATED, TEMPLATE_DESCRIPTION };

        public FileTemplate(StorageFile fileData)
        {
            this.fileData = fileData ?? throw new Exception(message: "error: fileData must not be null.");
            this.localFileName = fileData.DisplayName + fileData.FileType;
            setupManifest();
            allSet[(int)SetType.LOCAL_FILEDATA] = true;
            allSet[(int)SetType.LOCAL_FILENAME] = true;
        }

        public String getTemplateName()
        {
            return templateName;
        }

        public String getLocalFileName()
        {
            return this.localFileName;
        }

        public void setTemplateName(String templateName)
        {
            this.templateName = templateName;
            allSet[(int)SetType.TEMPLATE_NAME] = true;
        }

        public String getDateCreated()
        {
            return dateCreated;
        }

        public void setDateCreated(String dateCreated)
        {
            this.dateCreated = dateCreated;
            allSet[(int)SetType.DATE_CREATED] = true;
        }

        public String getDescription()
        {
            return templateDescription;
        }

        public void setDescription(String notes)
        {
            this.templateDescription = notes;
            allSet[(int)SetType.TEMPLATE_DESCRIPTION] = true;
        }

        /// <summary>
        /// file data to template should already be saved. saveTemplate() should only 
        /// modify the manifest file to recognize the file saved.
        /// </summary>
        public async void saveTemplate()
        {
            //check if all data members were set before trying to save template
            if (!isAllset())
                throw new System.NullReferenceException(
                    "All variables must be set to a value first before saving."
                    );
            //get manifest file to append additional template info to
            StorageFile templateManifest = 
                await ApplicationData.Current.LocalCacheFolder.GetFileAsync(MANIFEST_FILE);
            if (templateManifest == null)
                throw new System.NullReferenceException(
                    "templateManifest was set to null. manifest file has not been created."
                    );
            String line = this.localFileName + "," + dateCreated + "," + templateDescription + "\n";
            await FileIO.AppendLinesAsync(templateManifest, new List<String> { line });
            //create file to save actual template file data and save
            StorageFile localFile =
                await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(this.localFileName,
                CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(localFile, await FileIO.ReadTextAsync(this.fileData));
        }


        /// <summary>
        /// check if all data members are set
        /// </summary>
        /// <returns></returns>
        private bool isAllset()
        {
            foreach (bool b in allSet)
            {
                if (b is false)
                    return false;
            }
            return true;
        }


        /// <summary>
        /// create manifest file if not already created
        /// </summary>
        private async void setupManifest()
        {
            try
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
            catch(Exception e)
            {
                //nothing
            }
        }
    }
}
