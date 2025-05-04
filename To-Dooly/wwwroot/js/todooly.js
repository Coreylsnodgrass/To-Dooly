document.addEventListener("DOMContentLoaded", () => {
    // 1) AJAX Create Task
    const form = document.getElementById("create-task-form");
    form?.addEventListener("submit", async e => {
        e.preventDefault();

        // build a strongly-typed DTO payload
        const fd = new FormData(form);
        const payload = {
            projectId: Number(fd.get("projectId")),
            title: fd.get("title"),
            description: null,                      // add if you include a description input
            dueDate: fd.get("dueDate"),         // e.g. "2025-05-04"
            priority: Number(fd.get("priority"))
        };

        // POST to your API
        const resp = await fetch("/api/tasks", {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!resp.ok) {
            // log the server-side errors for debugging
            console.error(await resp.json());
            document.getElementById("create-task-error").textContent =
                "Could not save task.";
            return;
        }

        // reload the task-list partial
        const listContainer = document.getElementById("task-list-container");
        const html = await fetch(
            `/Projects/TaskList?projectId=${payload.projectId}`
        ).then(r => r.text());
        listContainer.innerHTML = html;

        // clear form & errors
        form.reset();
        document.getElementById("create-task-error").textContent = "";
    });

    // 2) AJAX Toggle Complete
    document
        .getElementById("task-list-container")
        ?.addEventListener("change", async e => {
            if (!e.target.classList.contains("toggle-complete")) return;

            const checkbox = e.target;
            const tr = checkbox.closest("tr");
            const id = Number(tr.getAttribute("data-task-id"));

            const resp = await fetch(`/api/tasks/${id}`, {
                method: "PUT",
                credentials: "same-origin",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ id: id, isComplete: checkbox.checked })
            });

            if (!resp.ok) {
                document.getElementById("toggle-error").textContent =
                    "Could not update status.";
                checkbox.checked = !checkbox.checked;
            } else {
                document.getElementById("toggle-error").textContent = "";
                tr.remove();  // remove completed from the list
            }
        });
});
