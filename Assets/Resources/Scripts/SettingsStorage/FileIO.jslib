var FileIO = {
  saveSettingsToLocalStorage : function(data) {
    localStorage.setItem("settings", Pointer_stringify(data));
  },

  loadSettingsFromLocalStorage : function() {
    var returnStr = localStorage.getItem("settings");
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  removeSettingsFromLocalStorage : function(key) {
    localStorage.removeItem("settings");
  },

  hasSettingsInLocalStorage : function() {
    if (localStorage.getItem("settings")) {
      return 1;
    }
    else {
      return 0;
    }
  }
};

mergeInto(LibraryManager.library, FileIO);;