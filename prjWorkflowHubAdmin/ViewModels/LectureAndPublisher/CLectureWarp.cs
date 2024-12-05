using prjWorkflowHubAdmin.ContextModels;

namespace prjWorkflowHubAdmin.ViewModels.LectureAndPublisher
{
    public class CLectureWarp
    {
        private TLecture _lec;
        private TPublisher _pub;
        public CLectureWarp()
        {
            _lec = new TLecture();
            _pub = new TPublisher();

        }
        public TLecture lec { get { return _lec; } set { _lec = value; } }
        public TPublisher pub { get { return _pub; } set { _pub = value; } }
        public string FPubName { get { return _pub.FPubName; } set { _pub.FPubName = value; } }

        public int FLectureId { get { return _lec.FLectureId; } set { _lec.FLectureId = value; } }

        public string FLecName { get { return _lec.FLecName; } set { _lec.FLecName = value; } }

        public int? FPublisherId { get { return _lec.FPublisherId; } set { _lec.FPublisherId = value; } }

        public byte[] FLecImage { get { return _lec.FLecImage; } set { _lec.FLecImage = value; } }

        public decimal? FLecPrice { get { return _lec.FLecPrice; } set { _lec.FLecPrice = value; } }

        public decimal? FLecPoints { get { return _lec.FLecPoints; } set { _lec.FLecPoints = value; } }

        public string FLecDescription { get { return _lec.FLecDescription; } set { _lec.FLecDescription = value; } }

        public bool? FOnline { get { return _lec.FOnline; } set { _lec.FOnline = value; } }

        public string FLecDate { get { return _lec.FLecDate; } set { _lec.FLecDate = value; } }

        public string FLecLocation { get { return _lec.FLecLocation; } set { _lec.FLecLocation = value; } }

        public int? FLecLimit { get { return _lec.FLecLimit; } set { _lec.FLecLimit = value; } }

        public string FLink { get { return _lec.FLink; } set { _lec.FLink = value; } }
    }
}
