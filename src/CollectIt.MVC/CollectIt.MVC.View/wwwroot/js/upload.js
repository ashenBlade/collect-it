function upload(){
    const input = document.querySelector('#file')
    const preview = document.createElement('div')
    
    preview.classList.add('preview')
    
    input.insertAdjacentElement('afterend', preview)
    
    const changeHandler = event => {
        if (!event.target.files.length){
            return
        }
        
        const file = Array.from(event.target.files)[0];
        
        const reader = new FileReader()
        
        reader.onload = ev =>{
            const src = ev.target.result
            preview.insertAdjacentHTML('afterbegin', `
            <img src="${src}" alt="${file.name}"/>
            `)
        }
        
        reader.readAsDataURL(file)
    }
}