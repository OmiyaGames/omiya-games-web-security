mergeInto(LibraryManager.library, {
  RedirectTo: function (url) {
    window.top.location.replace("'" + url + "'");
  },
});
