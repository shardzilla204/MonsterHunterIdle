using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class ItemBoxSearch : LineEdit
{
	[Signal]
	public delegate void FilterChangedEventHandler(Array<Material> materialsFound);
	
	public override void _Ready()
	{
		TextChanged += OnSearchTextChanged;
	}

	// * START - Signal Methods
	private void OnSearchTextChanged(string text)
	{
		string materialToFind = text.Trim();
		Array<Material> materialsFound = FindMaterials(materialToFind.ToUpper());
		EmitSignal(SignalName.FilterChanged, materialsFound);
	}
	// * END - Signal Methods

	private Array<string> GetKeywords()
	{
		IEnumerable<Material> distinctMaterials = ItemBox.Materials.Distinct();
		Array<string> keywords = new Array<string>();
		foreach (Material uniqueMaterial in distinctMaterials)
		{
			List<string> splitString = uniqueMaterial.Name.Split(' ').ToList();
			List<string> uppercaseKeywords = UppercaseKeywords(splitString);

			keywords.AddRange(uppercaseKeywords);
		}
		return keywords;
	}

	private Array<Material> FindMaterials(string keyword)
	{
		List<Material> distinctMaterials = ItemBox.Materials.Distinct().ToList();
		List<Material> materials = distinctMaterials.FindAll(uniqueMaterial => uniqueMaterial.Name.ToUpper().Contains(keyword));

		Array<Material> materialsFound = [.. materials]; // Convert From System.Collections.Generic.List To Godot.Collections.Array
		return materialsFound;
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
