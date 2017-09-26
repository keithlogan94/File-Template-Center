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

        public async void saveTemplate()
        {
            /*
             JSON FORMAT
             {
                [
                    { "templateName" : "template name" , "dateCreated" : "17 September 2017, Monday 15:46" , "notes" : "notes that user set" , "fileData" : "file-with-data.dat" },
                    {"templateName":"template name","dateCreated":"17 September 2017, Monday 15:46", "notes":"notes that user set","fileData":"file-with-data.dat"},
                    {"templateName":"template name","dateCreated":"17 September 2017, Monday 15:46", "notes":"notes that user set","fileData":"file-with-data.dat"},
                    {"templateName":"template name","dateCreated":"17 September 2017, Monday 15:46", "notes":"notes that user set","fileData":"file-with-data.dat"},
                    {"templateName":"template name","dateCreated":"17 September 2017, Monday 15:46", "notes":"notes that user set","fileData":"file-with-data.dat"}
                ]
            }
             */
            StorageFile templateManifest = 
                await ApplicationData.Current.LocalCacheFolder.GetFileAsync(MANIFEST_FILE);
            if (templateManifest == null)
                throw new System.NullReferenceException(
                    "templateManifest was set to null. manifest file has not been created."
                    );
            String templateManifestContents = await FileIO.ReadTextAsync(templateManifest);
            JsonValue json = JsonValue.Parse(templateManifestContents);
            JsonArray templateFiles = new JsonArray();
            foreach (var obj in json.GetArray())
                templateFiles.Add(obj);
            JsonObject manifestData = new JsonObject();
            //manifestData.Set
            if (!isAllset())
                throw new System.NullReferenceException(
                    "All variables must be set to a value first before saving."
                    );
            String templateName = data.DisplayName;
            StorageFile localFile = 
                await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(templateName,
                CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(localFile, await FileIO.ReadTextAsync(data));
        }

        private bool isAllset()
        {
            foreach (bool b in allSet)
            {
                if (b is false)
                    return false;
            }
            return true;
        }

        private async void setupManifest()
        {
            StorageFile manifest = 
                await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(
                    MANIFEST_FILE, 
                    CreationCollisionOption.FailIfExists
                    );
            if (manifest != null)
            {
                await FileIO.WriteTextAsync(manifest, "{[]}");
            }
        }
    }
}
