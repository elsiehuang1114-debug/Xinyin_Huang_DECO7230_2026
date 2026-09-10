using UnityEngine;

/// <summary>
/// Sits on the ChapterStations parent GameObject.
/// Tracks all 4 ChapterStation instances and enforces single-selection highlight.
/// Selection index state lives in ChapterManager — this class manages only visuals.
/// </summary>
public class ChapterStationController : MonoBehaviour
{
    [Header("Stations")]
    [Tooltip("All four ChapterStation components, assigned in order (0-3).")]
    [SerializeField] private ChapterStation[] _stations;

    /// <summary>
    /// Highlight <paramref name="selected"/> and clear the highlight on every other station.
    /// Pass null to deselect all.
    /// </summary>
    public void SetSelected(ChapterStation selected)
    {
        if (_stations == null) return;
        foreach (var station in _stations)
        {
            if (station != null)
                station.SetSelected(station == selected);
        }
    }
}
