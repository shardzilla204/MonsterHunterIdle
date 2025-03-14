using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MonsterHunterIdle;

public partial class ItemBoxSearch : LineEdit
{
	public ItemBoxDisplay ItemBoxDisplay;

	public override void _Ready()
	{
		TextChanged += SearchMaterial;
	}

	private void SearchMaterial(string text)
	{
		string materialToFind = text.Trim();
		if (materialToFind == "")
		{
			ItemBoxDisplay.FilterMaterialLogs(null);
			return;
		}

		List<dynamic> materialsFound = FindMaterials(materialToFind.ToUpper());
		
		ItemBoxDisplay.FilterMaterialLogs(materialsFound);
	}

	private List<string> GetKeywords()
	{
		IEnumerable<dynamic> uniqueMaterials = GameManager.Instance.ItemBox.Materials.Distinct();
		List<string> keywords = new List<string>();
		foreach (dynamic material in uniqueMaterials)
		{
			string materialName = material.Name.ToString();
			List<string> splitString = materialName.Split(' ').ToList();
			List<string> uppercaseKeywords = UppercaseKeywords(splitString);

			keywords.AddRange(uppercaseKeywords);
		}
		return keywords;
	}

	private List<dynamic> FindMaterials(string keyword)
	{
		List<dynamic> uniqueMaterials = GameManager.Instance.ItemBox.Materials.Distinct().ToList();
		return uniqueMaterials.FindAll(uniqueMaterial => uniqueMaterial.Name.ToUpper().Contains(keyword));
	}

	private List<string> UppercaseKeywords(List<string> keywords)
	{
		List<string> uppercase = new List<string>();
		foreach (string keyword in keywords)
		{
			uppercase.Add(keyword.ToUpper());
		}
		return uppercase;
	}
}
