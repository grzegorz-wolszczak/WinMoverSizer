using System;
using System.Collections.Generic;
using System.Linq;
using WinMoverSizer.WinApi;

namespace WinMoverSizer.Models;

public sealed class KeysPressed
{
   private readonly HashSet<KeyMetadata> _keysPressed = new();
   public KeysPressed(params KeyMetadata[] keys)
   {
      if (keys is null)
      {
         throw new ApplicationException("Key combination cannot be null");
      }

      foreach (var key in keys)
      {
         if (_keysPressed.Contains(key))
         {
            throw new ApplicationException($"Key '{key}' already exists in pressed key set");
         }

         _keysPressed.Add(key);
      }
   }

   public bool AreKeysPressed(HashSet<int> keyCombination)
   {
      if (keyCombination is null)
      {
         throw new ApplicationException("Key combination cannot be null");
      }

      return _keysPressed.Select(x=>x.Code).ToHashSet().SetEquals(keyCombination);
   }

   public string AsString()
   {
      return string.Join("+", _keysPressed.Select(e => e.LongName).ToList());
   }
}
