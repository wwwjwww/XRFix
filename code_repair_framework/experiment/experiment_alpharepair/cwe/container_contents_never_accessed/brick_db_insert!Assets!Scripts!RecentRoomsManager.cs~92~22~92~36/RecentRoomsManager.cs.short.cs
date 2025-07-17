using System.Globalization;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using System.IO;
using TMPro;

    private void InitializeSavedRoomsList()
    {
        // BUG: Container contents are never accessed
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        //         List<string> roomNamesToLoad = new List<string>();

        //Remove or Commented-out the collection if it is no longer needed
        // FIXED CODE:
