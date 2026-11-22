document.addEventListener('DOMContentLoaded', () => {
    const addWorkoutBtn = document.getElementById('add-workout-btn');
    const workoutsContainer = document.getElementById('workouts-container');
    const noWorkoutsMessage = document.getElementById('no-workouts-message');
    const planStartDateInput = document.getElementById('StartDate');

    if (!workoutsContainer) {
        return;
    }

    /**
     * Updates visual numbering (chips and titles) for all workout cards, and maintains
     * contiguous 0-based indexing on new workouts for ASP.NET Core Model Binding.
     */
    function updateWorkoutsState() {
        const cards = workoutsContainer.querySelectorAll('.workout-input-card');
        const collectionName = workoutsContainer.dataset.newWorkoutsPrefix || 'Workouts';
        let newWorkoutIndex = 0;

        cards.forEach((card, index) => {
            // Update visual index chip (1, 2, 3...)
            const chip = card.querySelector('.index-chip');
            if (chip) {
                chip.textContent = index + 1;
            }

            // Update title
            const title = card.querySelector('.workout-input-header b');
            if (title) {
                if (card.dataset.isExisting === 'true') {
                    title.textContent = `Workout ${index + 1}`;
                } else {
                    title.textContent = 'New workout';
                }
            }

            // Only update indices for new workouts (existing ones only send KeptWorkoutIds)
            if (card.dataset.isExisting === 'false') {
                const currentIndex = newWorkoutIndex++;

                card.querySelectorAll('input, select, textarea').forEach(input => {
                    if (input.name) {
                        const match = input.name.match(/(?:Workouts|NewWorkouts)\[\d+\]\.(.*)/);
                        const fieldName = match ? match[1] : input.name;
                        input.name = `${collectionName}[${currentIndex}].${fieldName}`;
                        input.id = `${collectionName}_${currentIndex}__${fieldName}`;
                    }
                });

                card.querySelectorAll('label[for]').forEach(label => {
                    const forAttr = label.htmlFor;
                    const match = forAttr.match(/(?:Workouts|NewWorkouts)_\d+__(.*)/);
                    const fieldName = match ? match[1] : forAttr;
                    label.htmlFor = `${collectionName}_${currentIndex}__${fieldName}`;
                });

                card.querySelectorAll('span[data-valmsg-for]').forEach(span => {
                    const valMsgFor = span.getAttribute('data-valmsg-for');
                    if (valMsgFor) {
                        const match = valMsgFor.match(/(?:Workouts|NewWorkouts)\[\d+\]\.(.*)/);
                        const fieldName = match ? match[1] : valMsgFor;
                        span.setAttribute('data-valmsg-for', `${collectionName}[${currentIndex}].${fieldName}`);
                    }
                });
            }
        });

        // Toggle the empty state message
        if (noWorkoutsMessage) {
            noWorkoutsMessage.hidden = cards.length > 0;
        }
    }

    /**
     * Clones a new workout card element from the HTML5 <template> in Razor.
     */
    function createWorkoutCard(defaultDate) {
        const template = document.getElementById('workout-card-template');
        if (!template) {
            return null;
        }

        const card = template.content.firstElementChild.cloneNode(true);
        if (defaultDate) {
            const dateInput = card.querySelector('input[type="date"]');
            if (dateInput) {
                dateInput.value = defaultDate;
            }
        }

        return card;
    }

    /**
     * Adds a new workout card to the container.
     */
    function addWorkout() {
        const defaultDate = planStartDateInput?.value || '';

        const newCard = createWorkoutCard(defaultDate);
        if (!newCard) {
            return;
        }

        workoutsContainer.appendChild(newCard);
        updateWorkoutsState();

        // Re-parse unobtrusive validation for newly added inputs
        const form = workoutsContainer.closest('form');
        if (form && window.jQuery && window.jQuery.validator && window.jQuery.validator.unobtrusive) {
            window.jQuery(form).removeData('validator');
            window.jQuery(form).removeData('unobtrusiveValidation');
            window.jQuery.validator.unobtrusive.parse(form);
        }

        // Smoothly scroll to reveal the newly added workout card along with the action buttons (Save/Cancel)
        requestAnimationFrame(() => {
            const form = workoutsContainer.closest('form');
            const submitBtn = form ? form.querySelector('button[type="submit"]') : null;
            const formActions = submitBtn ? submitBtn.closest('.flex-row') : null;
            const targetElement = formActions || newCard;

            targetElement.scrollIntoView({ behavior: 'smooth', block: 'end' });
        });
    }

    /**
     * Removes the workout card from the DOM and updates the remaining indices.
     */
    function removeWorkout(button) {
        const card = button.closest('.workout-input-card');
        if (card) {
            card.remove();
            updateWorkoutsState();
        }
    }

    // Event listener for adding workouts
    if (addWorkoutBtn) {
        addWorkoutBtn.addEventListener('click', addWorkout);
    }

    // Event delegation for removing workouts
    workoutsContainer.addEventListener('click', event => {
        const removeBtn = event.target.closest('.remove-workout-btn');
        if (removeBtn) {
            removeWorkout(removeBtn);
        }
    });

    // Ensure indices and chips are synchronized on initial page load
    updateWorkoutsState();
});
