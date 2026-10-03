
setInterval(() => {

  window.addEventListener('load', () => requestAnimationFrame(() => {
    const addOptionBtn = document.querySelector('#add-option');
    const addOptionsBtn = document.querySelector('#add-options');
    const removeOptionBtn = document.querySelector('#remove-option');
    const removeOptionsBtn = document.querySelector('#remove-options');
    const select = window.HSSelect?.getInstance('#hs-select-with-dynamic-options');

    addOptionBtn.addEventListener('click', () => {
      console.log(select);
      select.addOption({
        title: "Jannete Atkinson",
        val: "4",
        options: {
          icon: `<img class="inline-block size-6 rounded-full" src="../assets/images/faces/12.jpg" alt="Jannete Atkinson" >`
        }
      });
    });

    addOptionsBtn.addEventListener('click', () => {
      select.addOption([
        {
          title: "Kyle Peterson",
          val: "5",
          options: {
            icon: `<img class="inline-block size-6 rounded-full" src="../assets/images/faces/13.jpg" alt="Kyle Peterson">`
          }
        },
        {
          title: "Brad Cooper",
          val: "6",
          options: {
            icon: `<img class="inline-block size-6 rounded-full" src="../assets/images/faces/14.jpg" alt="Brad Cooper">`
          }
        },
        {
          title: "Linette Johnson",
          val: "7",
          options: {
            icon: `<img class="inline-block size-6 rounded-full" src="../assets/images/faces/15.jpg" alt="Brad Cooper">`
          }
        }
      ]);
    });

    removeOptionBtn.addEventListener('click', () => {
      select.removeOption("4");
    });

    removeOptionsBtn.addEventListener('click', () => {
      select.removeOption(["5", "6", "7"]);
    });

  }));

 
}, 1000);

