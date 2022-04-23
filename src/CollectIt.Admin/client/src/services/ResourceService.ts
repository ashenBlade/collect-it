import {serverAddress as server} from "../constants";

export class ResourceService {

    async getImagesPagedAsync({pageSize, pageNumber}: {pageSize: number, pageNumber: number}) {
        if (pageSize < 1 || pageNumber < 1) {
            throw new Error('Page size and page number must be positive');
        }
        const images = await fetch(`${server}/api/v1/images`, {
            body: JSON.stringify({

            })
        })
    }
}