using System.Collections;
using System.Collections.Generic;

public static class ImportedModelTracker
{
	static List<ImportedModel> importedModels = new List<ImportedModel>();
	public static List<ImportedModel> ImportedModels {
		get {
			return new List<ImportedModel>(importedModels);
		}
		private set {
			importedModels = value;
		}
	}

	public delegate void AddAction(ImportedModel obj);
	public static event AddAction OnAdded;
	
	public delegate void RemoveAction(ImportedModel obj);
	public static event RemoveAction OnRemove;

	public static void AddObject(ImportedModel obj) {
		importedModels.Add(obj);
		if (OnAdded != null)
			OnAdded(obj);
	}
	public static void RemoveObject(ImportedModel obj) {
		importedModels.Remove(obj);
		if (OnRemove != null)
			OnRemove(obj);
	}
}