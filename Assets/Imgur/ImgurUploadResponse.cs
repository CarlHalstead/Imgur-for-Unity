using System;
using System.Collections.Generic;

[Serializable]
public class ImgurUploadResponse
{
    public Data data;
    public bool success;
    public int status;

    [Serializable]
    public class Data
    {
        public string id;
        public string title;
        public string description;
        public int datetime;
        public string type;
        public bool animated;
        public int width;
        public int height;
        public int size;
        public int views;
        public int bandwidth;
        public string vote;
        public bool? favorite;
        public bool? nsfw;
        public string section;
        public string account_url;
        public int account_id;
        public bool is_ad;
        public bool is_most_viral;
        public bool has_sound;
        public List<string> tags;
        public int ad_type;
        public string ad_url;
        public bool in_gallery;
        public string deletehash;
        public string name;
        public string link;
    }
}
