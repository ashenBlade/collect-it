import React, {useState} from 'react';
import Image from "../../entities/image";
import InputBlock from "../../editBlocksComponents/editInputBlock/InputBlock";
import DeleteButton from "../../UIComponents/deleteButtonComponent/DeleteButton";

const EditVideo = () => {
    const video = new Image(1, 'New', new Date(), 'Dog',
        ["red", "green", "blue"], 'jpg', 1);

    const [name,setName] = useState<string|undefined>(video.name)

    const saveChanges = (e: React.ChangeEvent) => {
        e.preventDefault()
        console.log(name);
    }

    const cv = (name?: string) => {
        setName(name)
    }

    return (
        <div className='align-items-center justify-content-center shadow border col-6 mt-4 m-auto d-block rounded'>
            <form className='col-12 p-3'>
                <p className='h2 text-center'>Edit Resource {name}</p>

                <div className='ms-4 row'>
                    <div className='h6 d-block' style={{width: "50%"}}>
                        ID: {video.id}
                    </div>
                    <div className='h6 d-block' style={{width: "50%"}}>
                        Filename: {video.filename}
                    </div>
                    <div className='h6 d-block' style={{width: "50%"}}>
                        Extension: {video.extension}
                    </div>
                    <div className='h6 d-block' style={{width: "50%"}}>
                        Owner ID: {video.ownerId}
                    </div>
                </div>

                <InputBlock id={video.id} label={"Name:"} onClickSaveButton={saveChanges} change={cv}>
                    {name}
                </InputBlock>
                <InputBlock id={video.id} label={"Tags:"} onClickSaveButton={saveChanges} change={cv}>
                    {video.tags.join(' ')}
                </InputBlock>

                <DeleteButton className='btn btn-danger justify-content-center my-2 hc-0 ms-4'></DeleteButton>
            </form>
        </div>
    );
};

export default EditVideo;