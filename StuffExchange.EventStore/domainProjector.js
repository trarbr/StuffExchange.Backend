// The domainProjection.
// Remember to run with --run-projections=all
// and enable emit for the projection
// and set mode as Continous
fromAll().when({
    'GiftAdded': handle,
    'TitleChanged': handle,
    'DescriptionUpdated': handle,
    'ImageAdded': handle,
    'CommentAdded': handle
});

function handle(state, ev) {
    linkTo("domainEvents", ev);
}
