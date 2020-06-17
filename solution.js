let n = Pos3(0,1,0); // long .x .y .z
let e = Pos3(1,0,0);
let s = Pos3(0,-1,0);
let w = Pos3(-1,0,0);
let u = Pos3(0,0,1);
let d = Pos3(0,0,-1);

log("equal " + (Pos3(0,1,0) == Pos3(0,1,1)));

let tunnel = function(dir) {
  log("tunnel " + dir);
  if (!move(dir)) {
    get(dir);
    let placed = false;
    if (!placed && dir != n) {
      placed = put(n);
    }
    if (!placed && dir != e) {
      placed = put(e);
    }
    if (!placed && dir != w) {
      placed = put(w);
    }
    if (!placed && dir != u) {
      placed = put(u);
    }
    if (!placed && dir != d) {
      placed = put(d);
    }
    if (!placed && dir != s) {
      placed = put(s);
    }
    move(dir);
  }
};
let tunnelto = function(target) {
  log("tunnelto " + target);
  do {
    let pos = getPos();
    let x = 0;
    if (pos.x > target.x) {
      x = -1;
    } else if (pos.x < target.x) {
      x = 1;
    }
    let y = 0;
    if (pos.y > target.y) {
      y = -1;
    } else if (pos.y < target.y) {
      y = 1;
    }
    let z = 0;
    if (pos.z > target.z) {
      z = -1;
    } else if (pos.z < target.z) {
      z = 1;
    }
    if (x == 0 && y == 0 && z == 0) {
      return;
    }
    tunnel(Pos3(x,y,z));
  } while (true);
};

for (let y = -2; y <= 2; y++) {
  for (let z = -2; z <= 2; z++) {
    for (let x = -2; x <= 2; x++) {
      tunnelto(Pos3(x,y,z));
      while (move(n)) {}
      get(n);
      tunnelto(Pos3(x,y+1,z)); // This would be bad under certain circumstances we're not triggering
      put(s);
    }
  }
}
