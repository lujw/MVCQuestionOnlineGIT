$(function () {
    var editorContent = null;
    KindEditor.ready(function (K) {
        editorContent = K.create('textarea[name="Content"]', {
            allowImageUpload: true,
            uploadJson: '/admin/UploadFile'
        });
    });


    $("#BtnSubmit").click(function () {
        editorContent.html(ReplaceKESpace(editorContent.html()));
        editorContent.sync();
    });
});