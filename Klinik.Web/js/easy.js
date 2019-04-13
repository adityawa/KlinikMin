$(document).ready(function () {
    //easy.start();
});
easy = {};
easy.dateFormatServer = "dd-MM-yyyy";// "MM-dd-yyyy";
easy.dateFormat = "dd-mm-yyyy";// "mm-dd-yyyy";


easy.alert = function (message, title) {
    if (title == null || title == "") title = "MMS";
    $('#popModalTitle').html(title);
    $('#popModalMessage').html(message).trigger('create');
    $('#popModal').modal('show');
}