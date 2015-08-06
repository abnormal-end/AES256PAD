namespace aes256pad
{
    class FileStatus
    {
        public bool IsNew
        {
            get
            {
                return FilePath == null;
            }
        }
        public string FilePath { get; set; }
        public bool IsEdited { get; set; }
    }
}
