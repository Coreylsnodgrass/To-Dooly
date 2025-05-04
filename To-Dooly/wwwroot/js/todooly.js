// wwwroot/js/todooly.js

document.addEventListener("DOMContentLoaded", () => {
  const inContainer   = document.getElementById("task-list-container");
  const compContainer = document.getElementById("completed-task-list-container");
  const projectId     = Number(document.getElementById("ProjectId")?.value || 0);

  async function reloadLists() {
    const [openHtml, compHtml] = await Promise.all([
      fetch(`/Projects/TaskList?projectId=${projectId}`).then(r => r.text()),
      fetch(`/Projects/CompletedTaskList?projectId=${projectId}`).then(r => r.text())
    ]);
    if (inContainer)   inContainer.innerHTML   = openHtml;
    if (compContainer) compContainer.innerHTML = compHtml;
  }

  document.body.addEventListener("click", async e => {
    // Toggle complete
    if (e.target.matches(".toggle-complete")) {
      const tr = e.target.closest("tr");
      const id = tr.dataset.taskId;
      document.querySelectorAll(".delete-error")
              .forEach(el => el.textContent = "");
      const resp = await fetch(`/api/tasks/${id}`, {
        method: "PUT",
        credentials: "same-origin",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ isComplete: e.target.checked })
      });
      if (!resp.ok) {
        e.target.checked = !e.target.checked;
      } else {
        await reloadLists();
      }
      return;
    }

      if (e.target.matches(".delete-task")) {
          e.preventDefault();

          const tr = e.target.closest("tr");
          const id = tr?.dataset.taskId;
          if (!id) return;                       

          const err = document.querySelector(".delete-error");
          if (err) err.textContent = "";

          const resp = await fetch(`/api/tasks/${id}`, {
              method: "DELETE",
              credentials: "same-origin"         
          });

          if (resp.ok) {
              await reloadLists();
          } else {
              if (err) err.textContent = "Could not delete task.";
              console.error(await resp.text());
          }
          return;
      }
  });
});
