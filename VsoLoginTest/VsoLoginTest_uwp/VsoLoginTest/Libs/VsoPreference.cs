using CommonData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

[assembly: Xamarin.Forms.Dependency(typeof(VsoPreferenceStore))]
public class VsoPreferenceStore : IVsoPreferenceStore
{
    const string preferenceFileName = "preference.xml";

    public VsoPreferenceStore()
    {
        _preference = new VsoPreference();
    }

    private VsoPreference _preference = null;
    public VsoPreference Preference
    {
        get
        {
            if (_preference == null)
            {
                _preference = new VsoPreference();
            }
            return _preference;
        }
    }

    public Task<bool> Load()
    {
        return Task.Factory.StartNew<bool>(() =>
        {
            try
            {
                Task<Stream> ts = ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(preferenceFileName);
                ts.Wait();
                using (Stream s = ts.Result)
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(VsoPreference));
                    VsoPreference pref = (VsoPreference)serializer.ReadObject(s);
                    pref.CopyTo(_preference);
                    _preference.IsLoaded = true;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        });
    }

    public Task<bool> Save()
    {
        return Task.Factory.StartNew<bool>(() =>
        {
            try
            {
                Task<StorageFile> tsf = ApplicationData.Current.LocalFolder.CreateFileAsync(preferenceFileName, CreationCollisionOption.ReplaceExisting).AsTask();
                tsf.Wait();
                Task<Stream> ts = tsf.Result.OpenStreamForWriteAsync();
                ts.Wait();
                using (Stream s = ts.Result)
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(VsoPreference));
                    serializer.WriteObject(s, _preference);
                    _preference.CleanDirty();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        });
    }

    public override string ToString()
    {
        return "VsoPreferenceStore(" + GetHashCode().ToString() + ")";
    }
}
