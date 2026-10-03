(function () {
    "use strict"

    dragula([document.querySelector('#new-tasks-draggable'), document.querySelector('#todo-tasks-draggable'), document.querySelector('#inprogress-tasks-draggable'), document.querySelector('#inreview-tasks-draggable'), document.querySelector('#completed-tasks-draggable')]);

    document.addEventListener("DOMContentLoaded", () => {
        setInterval(() => {
            let i = [
                document.querySelector('#new-tasks-draggable'),
                document.querySelector('#todo-tasks-draggable'),
                document.querySelector('#inprogress-tasks-draggable'),
                document.querySelector('#inreview-tasks-draggable'),
                document.querySelector('#completed-tasks-draggable')

            ]
            i.map((ele) => {
                if (ele) {
                    if (ele.children.length == 0) {
                        ele.classList.add("task-Null")
                        document.querySelector(`#${ele.getAttribute("data-view-btn")}`).nextElementSibling?.classList.add("d-none")
                    }
                    if (ele.children.length != 0) {
                        ele.classList.remove("task-Null")
                        document.querySelector(`#${ele.getAttribute("data-view-btn")}`).nextElementSibling?.classList.remove("d-none")
                    }
                }
            })
        }, 1000);
    })

})();