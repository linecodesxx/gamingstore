document.querySelectorAll("[data-autosubmit]").forEach((form) => {
    form.addEventListener("change", () => form.submit());
});
