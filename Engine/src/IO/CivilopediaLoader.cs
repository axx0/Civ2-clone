using Civ2engine.IO;
using Model.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Civ2engine;

public class CivilopediaLoader
{
    private static string _currentDescribePath = "";
    private static string _currentPediaPath = "";

    /// <summary>
    /// Get path to describe.txt and pedia.txt and update content from Describe.txt.
    /// </summary>
    public static void UpdateMapping(Ruleset? rules)
    {
        var describePath = rules != null ? Utils.GetFilePath("describe.txt", rules.Paths) : Utils.GetFilePath("describe.txt");
        if (describePath == _currentDescribePath || string.IsNullOrWhiteSpace(describePath)) return;

        var pediaPath = rules != null ? Utils.GetFilePath("pedia.txt", rules.Paths) : Utils.GetFilePath("pedia.txt");
        if (pediaPath == _currentPediaPath || string.IsNullOrWhiteSpace(pediaPath)) return;

        _currentPediaPath = pediaPath;
        _currentDescribePath = describePath;
        GetMappingIndexes(describePath);
    }

    public static List<int> AdvanceIndex { get; set; } = [];
    public static List<int> ImprovementIndex { get; set; } = [];
    public static List<int> WonderIndex { get; set; } = [];
    public static List<int> UnitIndex { get; set; } = [];
    public static List<int> TerrainIndex { get; set; } = [];
    public static List<int> GovermentIndex { get; set; } = [];
    public static List<int> ConceptIndex { get; set; } = [];

    /// <summary>
    /// Get mapping of section locations from describe.txt in order of entries in Rules file.
    /// </summary>
    /// <param name="filePath">Path to describe.txt.</param>
    public static void GetMappingIndexes(string filePath)
    {
        bool inTargetSection = false;
        bool endSearch = false;
        string[] sections = ["@@ADVANCE_INDEX", "@@IMPROVEMENT_INDEX", "@@WONDER_INDEX", "@@UNIT_INDEX", "@@TERRAIN_INDEX", "@@GOVERNMENT_INDEX"];
        List<int>[] indexes = [AdvanceIndex, ImprovementIndex, WonderIndex, UnitIndex, TerrainIndex, GovermentIndex];

        var i = 0;
        foreach (var line in File.ReadLines(filePath))
        {
            if (endSearch) break;

            if (line.StartsWith("@@"))
            {
                if (inTargetSection)
                    break;

                inTargetSection = line.Equals(sections[i], StringComparison.OrdinalIgnoreCase);
                continue;
            }

            if (inTargetSection)
            {
                var text = line.Split(',');
                var index = int.TryParse(text[0], out int val) ? val : 0;
                if (index != -2)
                {
                    indexes[i].Add(index);
                }
                else
                {
                    inTargetSection = false;
                    i++;
                    if (i == sections.Length)
                        endSearch = true;
                }
            }
        }
    }

    public static string GetDescription(Civilopedia pedia, int id)
    {
        var mappingIndex = pedia.InfoType switch
        {
            CivilopediaInfoType.Advances => AdvanceIndex,
            CivilopediaInfoType.Improvements => ImprovementIndex,
            CivilopediaInfoType.Wonders => WonderIndex,
            CivilopediaInfoType.Units => UnitIndex,
            CivilopediaInfoType.Terrains => TerrainIndex,
            CivilopediaInfoType.Governments => GovermentIndex,
            CivilopediaInfoType.Concepts => ConceptIndex,
            _ => throw new NotImplementedException()
        };

        var section = pedia.InfoType switch
        {
            CivilopediaInfoType.Advances => "@@ADVANCE_INDEX",
            CivilopediaInfoType.Improvements => "@@IMPROVEMENT_INDEX",
            CivilopediaInfoType.Wonders => "@@WONDER_INDEX",
            CivilopediaInfoType.Units => "@@UNIT_INDEX",
            CivilopediaInfoType.Terrains => "@@TERRAIN_INDEX",
            CivilopediaInfoType.Governments => "@@GOVERNMENT_INDEX",
            CivilopediaInfoType.Concepts => "@CONCEPT_DESCRIPTIONS",
            _ => throw new NotImplementedException()
        };

        string text = string.Empty;
        bool inSection = false;
        bool inDescription = false;
        bool endSearch = false;
        int sectionCount = 0;
        foreach (var line in File.ReadLines(_currentDescribePath))
        {
            if (endSearch) break;

            if (inDescription)
            {
                if (line.StartsWith('@'))
                {
                    endSearch = true;
                    break;
                }
                else
                {
                    text += line;
                }
            }

            if (line.StartsWith("@@") || line.StartsWith("@C"))
            {
                if (inSection)
                {
                    if (mappingIndex[id] == sectionCount)
                    {
                        inDescription = true;
                    }
                    sectionCount++;
                }
                else
                {
                    if (line.Equals(section))
                    {
                        inSection = true;
                    }
                }
            }
        }
        return text;
    }

    /// <summary>
    /// Get civilopedia text for improvements from pedia.txt.
    /// </summary>
    /// <param name="rulesId">Id of improvement in Rules.txt.</param>
    public static string GetPediaImprovementText(int rulesId)
    {
        string text = string.Empty;
        bool checkIfCorrectImprovement = false;
        bool inTargetSection = false;
        bool endSearch = false;
        foreach (var line in File.ReadLines(_currentPediaPath))
        {
            if (endSearch) break;

            if (line.StartsWith('@') && inTargetSection)
            {
                endSearch = true;
                break;
            }

            if (line.StartsWith("@;"))
            {
                checkIfCorrectImprovement = true;
                continue;
            }

            if (inTargetSection)
            {
                if (text == string.Empty)
                {
                    text = line;
                }
                else
                {
                    text += " " + line;
                }
            }

            if (checkIfCorrectImprovement)
            {
                inTargetSection = line.Equals("@PEDIAIMPROVE" + rulesId, StringComparison.OrdinalIgnoreCase);
                checkIfCorrectImprovement = false;
            }
        }
        return text;
    }

    /// <summary>
    /// Get civilopedia text for units from pedia.txt.
    /// </summary>
    /// <param name="flags">Unit flags from rules.txt.</param>
    public static string GetPediaUnitText(bool[] flags)
    {
        string text = string.Empty;
        bool checkIfCorrectSection = false;
        bool inTargetSection = false;
        int counter = 0;
        foreach (var line in File.ReadLines(_currentPediaPath))
        {
            if (counter == flags.Length) break;

            if (inTargetSection)
            {
                if (flags[counter])
                {
                    if (text == string.Empty)
                    {
                        text = line[1..];
                    }
                    else
                    {
                        text += " " + line[1..];
                    }
                }

                counter++;
                continue;
            }

            if (line.StartsWith('@'))
            {
                checkIfCorrectSection = true;
            }

            if (checkIfCorrectSection)
            {
                inTargetSection = line.Equals("@PEDIAUNITFACTS", StringComparison.OrdinalIgnoreCase);
                checkIfCorrectSection = false;
            }
        }
        return text;
    }

    public static List<string> ReadConceptsList()
    {
        List<string> concepts = [];
        bool inConceptsSection = false;
        foreach (var line in File.ReadLines(_currentDescribePath))
        {
            if (inConceptsSection)
            {
                if (line.StartsWith("@@"))
                {
                    concepts.Add(line[2..]);
                    continue;
                }
                else if (line.StartsWith('@'))
                {
                    break;
                }
            }

            if (line.StartsWith('@'))
            {
                if (line.Equals("@CONCEPT_DESCRIPTIONS"))
                {
                    inConceptsSection = true;
                }
            }
        }

        var alphabetically = concepts.OrderBy(s => s[0]).ToArray();
        foreach (var str in alphabetically)
            ConceptIndex.Add(concepts.FindIndex(s => s == str));

        return concepts;
    }
}
