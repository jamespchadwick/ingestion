namespace JamesPChadwick.Ingestion.Models
{
  public class FileLineData
  {
      public string? Registry { get; set; }

      public string? Module { get; set; }

      public string? Version { get; set; }

      public string? SubmissionType { get; set; }

      public override string ToString()
      {
          return $"{nameof(FileLineData)}: Registry={Registry}, Module={Module}, Version={Version}, SubmissionType={SubmissionType}";
      }
  }
}
