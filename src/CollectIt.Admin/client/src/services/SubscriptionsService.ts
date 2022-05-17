import authorizedFetch from "./AuthorizedFetch";
import { serverAddress as server } from "../constants";
import Subscription from "../components/entities/subscription";
import { ResourceType } from "../components/entities/resource-type";
import NotFoundError from "../utils/NotFoundError";

const baseApiPath = `${server}/api/v1/subscriptions`;

export default class SubscriptionsService {
    private static readonly fetch = authorizedFetch();

    static async getSubscriptionsPagedAsync(pageNumber: number, pageSize: number) {
        if (pageSize < 1 || pageNumber < 1) throw new Error('Page size and page number must be positive');
        const s = `${baseApiPath}?page_size=${pageSize}&page_number=${pageNumber}`;
        const response = await SubscriptionsService.fetch(s, {
            method: 'GET'
        });
        if (!response.ok) {
            throw new Error('Could not get subscriptions from server');
        }
        const json = await response.json();
        const subscriptions: Subscription[] = json.subscriptions;
        const totalCount = Number(json.totalCount);
        return {
            subscriptions,
            totalCount
        }
    }

    static async getSubscriptionsByResourceTypePagedAsync({pageSize, pageNumber, type}: {pageSize: number, pageNumber: number, type: ResourceType}) {
        if (pageSize < 1 || pageNumber < 1) {
            throw new Error('Page size and page number must be positive');
        }
        const s = `${baseApiPath}?page_size=${pageSize}&page_number=${pageNumber}&type=${type}`;
        console.log(`Sending request to : ${s}`);
        const response = await SubscriptionsService.fetch(s, {
            method: 'GET'
        });
        if (!response.ok) {
            throw new Error('Could not get subscriptions from server');
        }
        const json = await response.json();
        const subscriptions: Subscription[] = json.subscriptions;
        const totalCount = Number(json.totalCount);
        return {
            totalCount: totalCount,
            subscriptions: subscriptions
        };
    }

    static async getSubscriptionByIdAsync(id: number) {
        if (!Number.isInteger(id)) throw new Error(`Id must be integer. Given: ${id}`);
        const response = await SubscriptionsService.fetch(`${baseApiPath}/${id}`, {
            method: 'GET'
        });
        if (!response.ok) {
            if (response.status === 404) throw new NotFoundError();
            throw new Error('Something went wrong')
        }
        const subscription: Subscription = await response.json();
        return subscription;
    }

    static async updateSubscriptionBatchAsync(id: number, name: string, description: string) {
        if (!(name && description)) throw new Error('Name or description are not provided');
        if (name.length < 6) throw new Error(`Name length must be greater than 5. Given: ${name.length}`);
        if (description.length < 10) throw new Error(`Description length must be greater than 9. Given: ${description.length}`);
        if (!Number.isInteger(id)) throw new Error('Id must be integer');

        const response = await SubscriptionsService.fetch(`${baseApiPath}/${id}`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                name: name,
                description: description
            })
        });
        if (!response.ok) {
            if (response.status === 404) throw new NotFoundError();
            throw new Error('Error while updating subscription');
        }
    }

    static async deactivateSubscriptionAsync(subscriptionId: number) {
        if (!Number.isInteger(subscriptionId)) throw new Error(`Subscription Id must be integer. Given: ${subscriptionId}`);
        const response = await SubscriptionsService.fetch(`${baseApiPath}/${subscriptionId}/deactivate`, {
            method: 'POST'
        });
        if (!response.ok) {
            if (response.status === 404) throw new NotFoundError();
            console.error(response)
            throw new Error('Something went wrong')
        }
    }

    static async activateSubscriptionAsync(subscriptionId: number) {
        if (!Number.isInteger(subscriptionId)) throw new Error(`Subscription Id must be integer. Given: ${subscriptionId}`);
        const response = await SubscriptionsService.fetch(`${baseApiPath}/${subscriptionId}/activate`, {
            method: 'POST'
        });
        if (!response.ok) {
            if (response.status === 404) throw new NotFoundError();
            console.error(response)
            throw new Error('Something went wrong')
        }
    }

    static async createSubscriptionAsync(body: {
        name: string;
        description: string;
        monthDuration: number;
        price: number;
        resourceType: ResourceType;
        maxResourcesCount: number;
        restriction: any }) {

        let serialized = JSON.stringify(body);
        console.log(serialized)
        const response = await this.fetch(`${baseApiPath}`, {
            method: 'POST',
            body: serialized,
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            throw new Error('Could not create new subscription')
        }
        const subscription: Subscription = await response.json();
        return subscription;
    }
}