document.addEventListener("DOMContentLoaded", function () {
    let searchInput = document.getElementById("searchPatient");
    let patientList = document.getElementById("patientList");
    let hiddenInput = document.getElementById("selectedPatientId");

    searchInput.addEventListener("input", function () {
        let query = this.value.trim().toLowerCase();
        let items = document.querySelectorAll(".patient-item");
        let found = false;

        items.forEach(item => {
            let name = item.getAttribute("data-name").toLowerCase();
            if (name.includes(query)) {
                item.style.display = "block";
                found = true;
            } else {
                item.style.display = "none";
            }
        });

        patientList.style.display = found ? "block" : "none";
    });

    document.querySelectorAll(".patient-item").forEach(item => {
        item.addEventListener("click", function () {
            searchInput.value = this.getAttribute("data-name");
            hiddenInput.value = this.getAttribute("data-id");
            patientList.style.display = "none";
        });
    });

    document.addEventListener("click", function (event) {
        if (!searchInput.contains(event.target) && !patientList.contains(event.target)) {
            patientList.style.display = "none";
        }
    });
});
