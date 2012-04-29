namespace itsLib
{
    public class Project
    {
        uint Id;
        Session Session;

        public Project(Session Session, uint Id)
        {
            this.Id = Id;
            this.Session = Session;
        }

        public void setActive()
        {
            Session.GetHttpWebRequest("/main.aspx?ProjectID=" + Id).GetResponse().Close();
        }

        public string DashboardPath
        {
            get
            {
                setActive();
                return "/Project/project.aspx";
            }
        }
    }
}