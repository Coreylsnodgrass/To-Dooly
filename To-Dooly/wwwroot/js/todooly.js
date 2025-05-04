document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("create-task-form");
    const inContainer = document.getElementById("task-list-container");
    const compContainer = document.getElementById("completed-task-list-container");
    const projectId = Number(form.querySelector('input[name="projectId"]').value);

    // helper to reload both open & completed task lists
    async function reloadLists() {
        const [incHtml, compHtml] = await Promise.all([
            fetch(`/Projects/TaskList?projectId=${projectId}`).then(r => r.text()),
            fetch(`/Projects/CompletedTaskList?projectId=${projectId}`).then(r => r.text())
        ]);
        inContainer.innerHTML = incHtml;
        compContainer.innerHTML = compHtml;
    }

    // Create new task via AJAX
    form?.addEventListener("submit", async e => {
        e.preventDefault();

        const fd = new FormData(form);
        const payload = {
            projectId: projectId,
            title: fd.get("title"),
            description: null,               // add if you include a description field
            dueDate: fd.get("dueDate"),
            priority: Number(fd.get("priority"))
        };

        const resp = await fetch("/api/tasks", {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!resp.ok) {
            console.error(await resp.json());
            document.getElementById("create-task-error").textContent = "Could not save.";
            return;
        }

        form.reset();
        await reloadLists();
    });

    // Delegate clicks for both toggling complete and deleting
    document.body.addEventListener("click", async e => {
        // 1) Mark complete
        if (e.target.matches(".toggle-complete")) {
            const tr = e.target.closest("tr");
            const id = tr.getAttribute("data-task-id");

            const resp = await fetch(`/api/tasks/${id}`, {
                method: "PUT",
                credentials: "same-origin",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ isComplete: true })
            });

            if (!resp.ok) {
                document.getElementById("toggle-error").textContent = "Could not complete.";
                return;
            }
            await reloadLists();
            return;
        }

        // 2) Delete task
        if (e.target.matches(".delete-task")) {
            const tr = e.target.closest("tr");
            const id = tr.getAttribute("data-task-id");

            const resp = await fetch(`/api/tasks/${id}`, {
                method: "DELETE",
                credentials: "same-origin"
            });

            if (!resp.ok) {
                document.getElementById("delete-error").textContent = "Could not delete.";
                return;
            }
            await reloadLists();
            return;
        }
    });
});
