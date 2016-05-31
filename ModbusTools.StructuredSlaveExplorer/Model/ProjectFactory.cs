using System.IO;
using Newtonsoft.Json;

namespace ModbusTools.StructuredSlaveExplorer.Model
{
    public static class ProjectFactory
    {
        public static void SaveProject(ProjectModel project, string path)
        {
            var data = JsonConvert.SerializeObject(project);

            File.WriteAllText(path, data);
        }

        public static ProjectModel LoadProject(string path)
        {
            var data = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<ProjectModel>(data);
        }
    }
}
