import {BadRequestException, Body, Controller, Get, Param, Post} from '@nestjs/common';
import {SubscriptionsService} from "./subscriptions.service";
import {Authorize} from "../auth/jwt-auth.guard";
import {AuthorizeAdmin} from "../auth/admin-jwt-auth.guard";
import {CreateSubscriptionDto} from "./dto/create-subscription.dto";
import CreationException from "../common/creation.exception";

@Authorize()
@Controller('api/v1/subscriptions')
export class SubscriptionsController {
    constructor(private subscriptionsService: SubscriptionsService) { }

    @Get(':subscriptionId')
    async getSubscriptionById(@Param('subscriptionId') subscriptionId: number) {
        return await this.subscriptionsService.getSubscriptionById(subscriptionId);
    }

    @AuthorizeAdmin()
    @Post('')
    async createSubscription(@Body() dto: CreateSubscriptionDto) {
        try {
            const subscription = await this.subscriptionsService.createSubscriptionAsync(
                dto.name,
                dto.description,
                dto.price,
                dto.monthDuration,
                dto.appliedResourceType,
                dto.maxResourcesCount,
                dto.active,
                dto.restriction?.restrictionType,
                dto.restriction?.authorId,
                dto.restriction?.sizeBytes,
                dto.restriction?.daysTo,
                dto.restriction?.daysAfter,
                dto.restriction?.tags
            );
            console.log(`Created subscription: ${JSON.stringify(subscription, null, 2)}`);
            return subscription;
        } catch (e) {
            if (e instanceof CreationException) {
                throw new BadRequestException({
                    message: e.message,
                    errors: e.errors
                })
            }
            throw new BadRequestException({
                message: 'Could not create subscription. Try later.',
                errors: []
            }, 'Something went wrong on server')
        }
    }

}
