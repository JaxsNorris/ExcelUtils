
using Common.Extensions;
using System;
using System.Runtime.Serialization;

namespace Common.Exceptions
{
    [Serializable]
    public class WorksheetNotFoundException : Exception
    {
        public string Fullpath { get; private set; }
        public string WorksheetName { get; private set; }

        public WorksheetNotFoundException(string fullpath, string worksheetName) : base($"Worksheet not found - {worksheetName}")
        {
            Fullpath = fullpath;
            WorksheetName = worksheetName;
        }

        protected WorksheetNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Fullpath = info.GetStringOrEmpty(nameof(Fullpath));
            WorksheetName = info.GetStringOrEmpty(nameof(WorksheetName));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
            info.AddValue(nameof(Fullpath), Fullpath, typeof(string));
            info.AddValue(nameof(WorksheetName), WorksheetName, typeof(string));
        }
    }
}
