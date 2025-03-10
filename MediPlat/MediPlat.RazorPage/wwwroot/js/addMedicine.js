document.addEventListener("DOMContentLoaded", function () {
    let medicineIndex = 1;

    // Check if window.medicines is available and is an array
    if (!window.medicines || !Array.isArray(window.medicines) || window.medicines.length === 0) {
        return;
    }

    document.getElementById("add-medicine").addEventListener("click", function () {

        const medicineEntry = document.createElement("div");
        medicineEntry.classList.add("medicine-entry", "d-flex", "gap-2", "align-items-center", "mt-2");

        let medicineDropdown = document.createElement("select");
        medicineDropdown.name = `AppointmentSlot.Medicines[${medicineIndex}].MedicineId`;
        medicineDropdown.classList.add("form-select");

        window.medicines.forEach(med => {
            let option = document.createElement("option");
            option.value = med.Id || med.id;
            option.textContent = med.Name || med.name;
            medicineDropdown.appendChild(option);
        });

        medicineEntry.appendChild(medicineDropdown);

        let dosageInput = document.createElement("input");
        dosageInput.type = "text";
        dosageInput.name = `AppointmentSlot.Medicines[${medicineIndex}].Dosage`;
        dosageInput.placeholder = "Liều lượng";
        dosageInput.classList.add("form-control");
        dosageInput.required = true;
        medicineEntry.appendChild(dosageInput);

        let instructionsInput = document.createElement("input");
        instructionsInput.type = "text";
        instructionsInput.name = `AppointmentSlot.Medicines[${medicineIndex}].Instructions`;
        instructionsInput.placeholder = "Hướng dẫn sử dụng";
        instructionsInput.classList.add("form-control");
        medicineEntry.appendChild(instructionsInput);

        let quantityInput = document.createElement("input");
        quantityInput.type = "number";
        quantityInput.name = `AppointmentSlot.Medicines[${medicineIndex}].Quantity`;
        quantityInput.placeholder = "Số lượng";
        quantityInput.classList.add("form-control");
        quantityInput.min = "1";
        quantityInput.required = true;
        medicineEntry.appendChild(quantityInput);

        let removeButton = document.createElement("button");
        removeButton.type = "button";
        removeButton.classList.add("btn", "btn-danger", "remove-medicine");
        removeButton.innerHTML = `<i class="fas fa-trash"></i>`;
        removeButton.addEventListener("click", function () {
            medicineEntry.remove();
        });

        medicineEntry.appendChild(removeButton);

        document.getElementById("medicines").appendChild(medicineEntry);
        medicineIndex++;
    });
});