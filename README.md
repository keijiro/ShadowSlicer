ShadowSlicer
------------

This is an example of special shadow collector that generates a slice of shadow
volumes with an invisible wall.

![screenshot](http://i.imgur.com/dYDQPBUl.png)

Unfortunately this implementation has several limitations and only useful in
some special situations.

- Doesn't support directional lights.
- Only supports the deferred shading rendering path.
- Doesn't support forward materials with deferred shading.
