    public static Eff<Unit> Run<TEx, TReq, TResp>(UpsertSpec<TEx, TReq, TResp> spec) =>
        EffectAsync(async () =>
        {
            var existing = spec.FindExisting();
            var req = existing.Match(
                ex => spec.BuildUpdate(ex),
                () => spec.BuildCreate());

            var resp = await spec.Send(req);
            spec.Commit(resp);
        });