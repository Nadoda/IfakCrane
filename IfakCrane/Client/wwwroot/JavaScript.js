function updateCirclePosition(circleElement, x, y) {
    circleElement.style.left = x + "px";
    circleElement.style.top = y + "px";
}
function highlightSelectedItem() {
    var selectBar = document.querySelector('.radzen-selectbar');
    var selectedItems = selectBar.querySelectorAll('.radzen-selectbar-item-selected');

    selectedItems.forEach(function (item) {
        item.style.backgroundColor = 'lightblue'; // Set your desired background color
    });
}

document.addEventListener('DOMContentLoaded', function () {
    highlightSelectedItem();

    // Attach the highlight function to the RadzenSelectBar's click event
    var selectBar = document.querySelector('.radzen-selectbar');
    selectBar.addEventListener('click', function () {
        setTimeout(function () {
            highlightSelectedItem();
        }, 0);
    });
});