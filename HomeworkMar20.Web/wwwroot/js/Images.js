$(() => {

    setInterval(() => {
        updateLikes();
    }, 1000);

    $("#like-button").on('click', function () {
        const id = $(this).data('image-id');
        $.post('/home/addlike', { id }, function (likes) {
            $("#like-button").prop('disabled', true);
            $("#likes-count").text(likes);
        });
    });

    function updateLikes() {
        const id = $("#image-id").val();
        $.get('/home/getlikesforimage', { id }, function (likes) {
            $("#likes-count").text(likes);
        });
    }
});